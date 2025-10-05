using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Customers.DTOs;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
{

    public async Task<Result<List<CustomerDto>>> Handle(
        GetCustomersQuery query,
        CancellationToken cancellationToken)
    {
        var customers = await context.Customers
        .Include(c => c.Vehicles)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

        return customers.ToDtos();
    }
}