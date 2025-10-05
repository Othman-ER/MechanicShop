namespace MechanicShop.Application.Customers.Commands.CreateCustomer;

public sealed record CreateVehicleCommand
(
    string Make,
    string Model,
    int Year,
    string LicensePlate
);
