using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand
(
    Guid CustomerId,
    string Name,
    string PhoneNumber,
    string Email,
    List<UpdateVehicleCommand> Vehicles

) : IRequest<Result<Updated>>;