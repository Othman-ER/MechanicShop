using MechanicShop.Application.Customers.DTOs;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Application.Workorders.DTOs;

public class WorkOrderListItemDto
{
    public Guid WorkOrderId { get; set; }
    public Guid? InvoiceId { get; set; }
    public VehicleDto Vehicle { get; set; } = default!;
    public string? Customer { get; set; }
    public string? Labor { get; set; }
    public WorkOrderState State { get; set; }
    public Spot Spot { get; set; }
    public DateTimeOffset StartAtUtc { get; set; }
    public DateTimeOffset EndAtUtc { get; set; }
    public List<string> RepairTasks { get; set; } = [];
}
