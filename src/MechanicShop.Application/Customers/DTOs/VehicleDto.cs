namespace MechanicShop.Application.Customers.DTOs;

public record VehicleDto
(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string LicensePlate
);
