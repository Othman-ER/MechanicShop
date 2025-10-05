using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Customers.DTOs;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(
    IAppDbContext context,
    ILogger<CreateCustomerCommandHandler> logger,
    HybridCache cache) :
    IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLower();

        if (await context.Customers.AnyAsync(c => c.Email == email, ct))
        {
            logger.LogWarning("Attempt to create a customer with an existing email: {Email}", email);
            return CustomerErrors.CustomerExists;
        }

        List<Vehicle> vehicles = [];

        foreach (var vehicleResult in command.Vehicles.Select(vehicle => 
                     Vehicle.Create(
                         id: Guid.NewGuid(),
                         make: vehicle.Make,
                         model: vehicle.Model,
                         year: vehicle.Year,
                         licensePlate: vehicle.LicensePlate
                     )))
        {
            if (vehicleResult.IsError)
            {
                logger.LogWarning("Invalid vehicle data provided for new customer: {Errors}", vehicleResult.Errors);
                return vehicleResult.Errors;
            }

            vehicles.Add(vehicleResult.Value);
        }

        var customerResult = Customer.Create(
            id: Guid.NewGuid(),
            name: command.Name.Trim(),
            email: email.Trim(),
            phoneNumber: command.PhoneNumber.Trim(),
            vehicles: vehicles
        );

        if (customerResult.IsError)
        {
            logger.LogWarning("Invalid customer data provided: {Errors}", customerResult.Errors);
            return customerResult.Errors;
        }

        context.Customers.Add(customerResult.Value);

        await context.SaveChangesAsync(ct);

        await cache.RemoveByTagAsync("customer", ct);

        logger.LogInformation("Customer created Successfully with ID: {CustomerId}", customerResult.Value.Id);

        return customerResult.Value.ToDto();
    }
}
