using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Labors.DTOs;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Labors.Queries.GetLabors;

public sealed record GetLaborsQuery() : ICachedQuery<Result<List<LaborDto>>>
{
    public string CacheKey => $"labors";
    public string[] Tags => ["labors"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
