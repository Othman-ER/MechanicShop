using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Workorders.DTOs;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Workorders.Queries.GetWorkOrderById;

public sealed record GetWorkOrderByIdQuery(Guid WorkOrderId) : ICachedQuery<Result<WorkOrderDto>>
{
    public string CacheKey => $"work-order:{WorkOrderId}";
    public string[] Tags => ["work-order"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
