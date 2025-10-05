using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Customers.DTOs;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery : ICachedQuery<Result<List<CustomerDto>>>
{
    public string CacheKey => "customers";
    public string[] Tags => ["customer"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
