using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.RepairTasks.Commands.CreateRepairTasks;

public sealed record CreateRepairTaskPartCommand(
    string Name,
    decimal Cost,
    int Quantity
) : IRequest<Result<Success>>;
