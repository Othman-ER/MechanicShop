using MechanicShop.Application.Common;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Workorders.Commands.UpdateWorkOrderRepairTasks;

public class UpdateWorkOrderRepairTasksCommandHandler(
    ILogger<UpdateWorkOrderRepairTasksCommandHandler> logger,
    IAppDbContext context,
    HybridCache cache,
    IWorkOrderPolicy workOrderValidator) :
    IRequestHandler<UpdateWorkOrderRepairTasksCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        UpdateWorkOrderRepairTasksCommand command,
        CancellationToken cancellationToken)
    {
        var workOrder = await context.WorkOrders
            .Include(w => w.RepairTasks)
            .FirstOrDefaultAsync(w => w.Id == command.WorkOrderId, cancellationToken);

        if (workOrder is null)
        {
            logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", command.WorkOrderId);

            return ApplicationErrors.WorkOrderNotFound;
        }

        if (command.RepairTaskIds.Length == 0)
        {
            logger.LogError("Empty RepairTaskIds list submitted.");

            return RepairTaskErrors.AtLeastOneRepairTaskIsRequired;
        }

        var requestedTasks = await context.RepairTasks
            .Where(t => command.RepairTaskIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        if (requestedTasks.Count != command.RepairTaskIds.Length)
        {
            var missingIds = command.RepairTaskIds.Except(requestedTasks.Select(t => t.Id)).ToArray();

            logger.LogError("One or more RepairTasks not found. {ids}", string.Join(", ", missingIds));

            return ApplicationErrors.RepairTaskNotFound;
        }

        var clearExistingResult = workOrder.ClearRepairTasks();

        if (clearExistingResult.IsError)
            return clearExistingResult;

        foreach (var task in requestedTasks)
        {
            var addRepairTaskResult = workOrder.AddRepairTask(task);

            if (addRepairTaskResult.IsError)
                return addRepairTaskResult;
        }

        var totalDuration = TimeSpan.FromMinutes(requestedTasks.Sum(x => (int)x.EstimatedDurationInMins));

        var newEndAt = workOrder.StartAtUtc + totalDuration;

        // Business validations
        if (workOrderValidator.IsOutsideOperatingHours(workOrder.StartAtUtc, totalDuration))
            return Error.Conflict("WorkOrder_Outside_OperatingHours", "WorkOrder timing exceeds business hours.");

        var spotCheckResult = await workOrderValidator.CheckSpotAvailabilityAsync(
            workOrder.Spot,
            workOrder.StartAtUtc,
            newEndAt,
            excludeWorkOrderId: workOrder.Id,
            ct: cancellationToken);

        if (spotCheckResult.IsError)
            return spotCheckResult.Errors;

        if (await workOrderValidator.IsLaborOccupied(workOrder.LaborId, workOrder.Id, workOrder.StartAtUtc, newEndAt))
            return ApplicationErrors.LaborOccupied;

        workOrder.UpdateTiming(workOrder.StartAtUtc, newEndAt);

        workOrder.AddDomainEvent(new WorkOrderCollectionModified());

        await context.SaveChangesAsync(cancellationToken);

        workOrder.AddDomainEvent(new WorkOrderCollectionModified());

        await cache.RemoveByTagAsync("work-order", cancellationToken);

        return Result.Updated;
    }
}