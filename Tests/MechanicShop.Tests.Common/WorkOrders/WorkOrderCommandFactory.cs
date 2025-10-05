using MechanicShop.Application.Workorders.Commands.CreateWorkOrder;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.tests.Common.WorkOrders;

public static class WorkOrderCommandFactory
{
    public static CreateWorkOrderCommand CreateCreateWorkOrderCommand(
        Spot? spot = null,
        Guid? vehicleId = null,
        DateTimeOffset? startAt = null,
        List<Guid>? repairTaskIds = null,
        Guid? laborId = null
    ) => new(
        spot ?? Spot.A,
        vehicleId ?? Guid.NewGuid(),
        startAt ?? DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(9),
        repairTaskIds ?? [Guid.NewGuid()],
        laborId ?? Guid.NewGuid()
    );
}
