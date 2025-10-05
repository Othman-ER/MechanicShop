using MechanicShop.Application.Common;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Workorders.DTOs;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Workorders.Queries.GetWorkOrderById;

public class GetWorkOrderByIdQueryHandler(
    ILogger<GetWorkOrderByIdQueryHandler> logger,
    IAppDbContext context) :
    IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
{
    public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery query, CancellationToken ct)
    {
        var workOrder = await context.WorkOrders
            .AsNoTracking()
            .Include(a => a.RepairTasks)
                .ThenInclude(a => a.Parts)
            .Include(a => a.Labor)
            .Include(a => a.Vehicle!)
                .ThenInclude(v => v.Customer)
            .Include(a => a.Invoice)
            .FirstOrDefaultAsync(a => a.Id == query.WorkOrderId, ct);

        if (workOrder is null)
        {
            logger.LogWarning("WorkOrder with id {WorkOrderId} was not found", query.WorkOrderId);

            return ApplicationErrors.WorkOrderNotFound;
        }

        return workOrder.ToDto();
    }
}