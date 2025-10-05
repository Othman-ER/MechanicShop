using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.tests.Common.RepairTasks;

public static class PartFactory
{
    public static Result<Part> CreatePart(
        Guid? id = null,
        string? name = null,
        decimal? cost = null,
        int? quantity = null)
    {
        return Part.Create(
            id ?? Guid.NewGuid(),
            name ?? "Brake Pad",
            cost ?? 100,
            quantity ?? 2
        );
    }
}
