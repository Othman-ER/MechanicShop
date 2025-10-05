using FluentValidation;

namespace MechanicShop.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty()
            .WithMessage("Make is required.")
            .MaximumLength(50)
            .WithMessage("Make must not exceed 50 characters.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required.")
            .MaximumLength(50)
            .WithMessage("Model must not exceed 50 characters.");

        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .WithMessage("License Plate is required.")
            .MaximumLength(10)
            .WithMessage("License Plate must not exceed 10 characters.");
    }
}