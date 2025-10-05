using MechanicShop.Application.Billing.DTOs;
using MechanicShop.Application.Common;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Constants;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.billing;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Billing.Commands.IssueInvoice;

public class IssueInvoiceCommandHandler(
    ILogger<IssueInvoiceCommandHandler> logger,
    IAppDbContext context,
    HybridCache cache,
    TimeProvider datetime)
    : IRequestHandler<IssueInvoiceCommand, Result<InvoiceDto>>
{

    public async Task<Result<InvoiceDto>> Handle(IssueInvoiceCommand command, CancellationToken ct)
    {
        var workOrder = await context.WorkOrders
            .Include(w => w.Vehicle!)
            .ThenInclude(v => v.Customer)
            .Include(w => w.RepairTasks)
            .ThenInclude(rt => rt.Parts)
            .FirstOrDefaultAsync(w => w.Id == command.WorkOrderId, ct);

        if (workOrder is null)
        {
            logger.LogWarning("Invoice issuance failed. WorkOrder {WorkOrderId} not found.", command.WorkOrderId);

            return ApplicationErrors.WorkOrderNotFound;
        }

        if (workOrder.State != WorkOrderState.Completed)
        {
            logger.LogWarning("Invoice issuance rejected. WorkOrder {WorkOrderId} is not in completed.", command.WorkOrderId);

            return ApplicationErrors.WorkOrderMustBeCompletedForInvoicing;
        }

        Guid invoiceId = Guid.NewGuid();

        var lineItems = new List<InvoiceLineItem>();

        var lineNumber = 1;

        foreach (var (task, taskIndex) in workOrder.RepairTasks.Select((t, i) => (t, i + 1)))
        {
            var partsSummary = task.Parts.Any()
               ? string.Join(Environment.NewLine, task.Parts.Select(p => $"    • {p.Name} x{p.Quantity} @ {p.Cost:C}"))
               : "    • No parts";

            var lineDescription =
                $"{taskIndex}: {task.Name}{Environment.NewLine}" +
                $"  Labor = {task.LaborCost:C}{Environment.NewLine}" +
                $"  Parts:{Environment.NewLine}{partsSummary}";

            var totalPartsCost = task.Parts.Sum(p => p.Cost * p.Quantity);
            var totalTaskCost = task.LaborCost + totalPartsCost;

            var lineItemResult = InvoiceLineItem.Create(
                invoiceId: invoiceId,
                lineNumber: lineNumber++,
                description: lineDescription,
                quantity: 1,
                unitPrice: totalTaskCost
            );

            if (lineItemResult.IsError)
            {
                return lineItemResult.Errors;
            }

            lineItems.Add(lineItemResult.Value);
        }

        var subtotal = lineItems.Sum(x => x.LineTotal);

        var taxAmount = subtotal * MechanicShopConstants.TaxRate;

        var discountAmount = workOrder.Discount ?? 0m;

        var createInvoiceResult = Invoice.Create(
            id: invoiceId,
            workOrderId: workOrder.Id,
            items: lineItems,
            discountAmount: discountAmount,
            taxAmount: taxAmount,
            datetime: datetime
        );

        if (createInvoiceResult.IsError)
        {
            logger.LogWarning(
                "Invoice creation failed for WorkOrderId: {WorkOrderId}. Errors: {@Errors}",
                command.WorkOrderId,
                createInvoiceResult.Errors
            );

            return createInvoiceResult.Errors;
        }

        var invoice = createInvoiceResult.Value;

        await context.Invoices.AddAsync(invoice, ct);

        await context.SaveChangesAsync(ct);

        await cache.RemoveByTagAsync("invoice", ct);

        logger.LogInformation("Invoice {InvoiceId} issued for WorkOrder {WorkOrderId}.", invoice.Id, workOrder.Id);

        return invoice.ToDto();
    }
}
