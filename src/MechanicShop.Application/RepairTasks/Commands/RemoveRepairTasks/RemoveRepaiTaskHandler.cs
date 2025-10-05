using System.ComponentModel.DataAnnotations;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.RepairTasks.Commands.RemoveRepairTasks;

public sealed record RemoveRepairTaskCommand([Required] Guid RepairTaskId)
    : IRequest<Result<Deleted>>;
