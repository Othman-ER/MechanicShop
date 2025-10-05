using FluentValidation;

namespace MechanicShop.Application.Workorders.Queries.GetWorkOrderById;

public sealed class GetAppointmentByIdQueryValidator : AbstractValidator<GetWorkOrderByIdQuery>
{
    public GetAppointmentByIdQueryValidator()
    {
        RuleFor(request => request.WorkOrderId)
            .NotEmpty()
            .WithErrorCode("WorkOrderId_Is_Required")
            .WithMessage("WorkOrderId is required.");
    }
}
