using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.RepairTasks.DTOs;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.RepairTasks.Queries.GetRepairTasks;

public class GetRepairTasksQueryHandler(IAppDbContext context)
    : IRequestHandler<GetRepairTasksQuery, Result<List<RepairTaskDto>>>
{
    public async Task<Result<List<RepairTaskDto>>> Handle(GetRepairTasksQuery query, CancellationToken ct)
    {
        var repairTasks = await context.RepairTasks
            .Include(rt => rt.Parts)
            .AsNoTracking()
            .ToListAsync(ct);

        return repairTasks.ToDtos();
    }
}
