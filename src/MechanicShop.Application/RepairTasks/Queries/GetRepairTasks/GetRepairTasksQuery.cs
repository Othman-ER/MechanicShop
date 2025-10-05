using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.RepairTasks.DTOs;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.RepairTasks.Queries.GetRepairTasks;

public sealed record GetRepairTasksQuery : ICachedQuery<Result<List<RepairTaskDto>>>
{
    public string CacheKey => "repair-tasks";

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);

    public string[] Tags => ["repair-tasks"];
}
