using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Customers.DTOs;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid CustomerId) : ICachedQuery<Result<CustomerDto>>
{
    public string CacheKey => $"customer_{CustomerId}";

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);

    public string[] Tags => ["customer"];
}
