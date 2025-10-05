using MechanicShop.Application.Labors.DTOs;
using MechanicShop.Domain.Employees;

namespace MechanicShop.Application.Labors;

public static class LaborMapper
{
    public static LaborDto ToDto(this Employee employee) => new()
    {
        LaborId = employee.Id,
        Name = employee.FullName
    };


    public static List<LaborDto> ToDtos(this IEnumerable<Employee> entities) =>
        [.. entities.Select(l => l.ToDto())];
}
