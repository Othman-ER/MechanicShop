using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateVehicleCommand
(
    Guid? VehicleId,
    string Make,
    string Model,
    int Year,
    string LicensePlate

) : IRequest<Result<Updated>>;
