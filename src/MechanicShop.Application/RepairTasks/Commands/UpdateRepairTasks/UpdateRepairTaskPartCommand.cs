namespace MechanicShop.Application.RepairTasks.Commands.UpdateRepairTasks;

public sealed record UpdateRepairTaskPartCommand(
    Guid? PartId,
    string Name,
    decimal Cost,
    int Quantity
);
