namespace MechanicShop.Domain.WorkOrders.Events;

public sealed class WorkOrderCompleted : DomainEvent
{
    public Guid WorkOrderId { get; init; }
}