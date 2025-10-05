using MechanicShop.Application.RepairTasks.DTOs;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Application.RepairTasks;

public static class RepairTaskMapper
{
    public static RepairTaskDto ToDto(this RepairTask entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new RepairTaskDto
        {
            RepairTaskId = entity.Id,
            Name = entity.Name!,
            LaborCost = entity.LaborCost,
            TotalCost = entity.TotalCost,
            EstimatedDurationInMins = entity.EstimatedDurationInMins,
            Parts = entity.Parts.ToList().ConvertAll(ToDto)
        };
    }

    public static List<RepairTaskDto> ToDtos(this IEnumerable<RepairTask> entities) =>
        [.. entities.Select(e => e.ToDto())];


    public static PartDto ToDto(this Part entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new PartDto
        {
            PartId = entity.Id,
            Name = entity.Name!,
            Cost = entity.Cost,
            Quantity = entity.Quantity
        };
    }

    public static List<PartDto> ToDtos(this IEnumerable<Part> entities) =>
        [.. entities.Select(e => e.ToDto())];
}
