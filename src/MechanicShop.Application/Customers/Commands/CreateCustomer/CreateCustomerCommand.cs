using MechanicShop.Application.Customers.DTOs;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand (
    string Name,
    string Email,
    string PhoneNumber,
    List<CreateVehicleCommand> Vehicles
) : IRequest<Result<CustomerDto>>;