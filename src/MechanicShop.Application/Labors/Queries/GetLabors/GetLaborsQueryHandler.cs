using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Labors.DTOs;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Labors.Queries.GetLabors;

public class GetLaborsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLaborsQuery, Result<List<LaborDto>>>
{
    public async Task<Result<List<LaborDto>>> Handle(GetLaborsQuery query, CancellationToken ct)
        => await context.Employees
            .Where(employee => employee.Role == Role.Labor)
            .Select(employee => new LaborDto
            {
                LaborId = employee.Id,
                Name = employee.FullName
            })
            .ToListAsync(ct);

    // Old Methode
    // public async Task<Result<List<LaborDto>>> Handle(GetLaborsQuery query, CancellationToken ct)
    //     => (await _context.Employees
    //         .AsNoTracking()
    //         .Where(e => e.Role == Role.Labor)
    //         .ToListAsync(ct)).ToDtos();
}
