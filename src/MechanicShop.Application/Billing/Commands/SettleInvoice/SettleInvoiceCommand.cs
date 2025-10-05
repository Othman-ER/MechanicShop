using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Billing.Commands.SettleInvoice;

public sealed record SettleInvoiceCommand(Guid InvoiceId) : IRequest<Result<Success>>;
