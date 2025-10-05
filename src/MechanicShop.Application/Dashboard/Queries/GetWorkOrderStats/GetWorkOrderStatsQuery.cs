using MechanicShop.Application.Dashboard.DTOs;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Dashboard.Queries.GetWorkOrderStats;

public sealed record GetWorkOrderStatsQuery(DateOnly Date) :
    IRequest<Result<TodayWorkOrderStatsDto>>;
