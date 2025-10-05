using FluentValidation;

namespace MechanicShop.Application.Workorders.Commands.AssignLabor;

public sealed class AssignLaborCommandValidator : AbstractValidator<AssignLaborCommand>
{
    public AssignLaborCommandValidator()
    {
        RuleFor(x => x.WorkOrderId)
         .NotEmpty()
         .WithErrorCode("WorkOrderId_Required")
         .WithMessage("WorkOrderId is required.");

        RuleFor(x => x.LaborId)
           .NotEmpty()
           .WithErrorCode("LaborId_Required")
           .WithMessage("LaborId is required.");
    }
}