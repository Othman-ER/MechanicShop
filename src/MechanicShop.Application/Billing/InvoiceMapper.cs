using MechanicShop.Application.Billing.DTOs;
using MechanicShop.Application.Customers;
using MechanicShop.Domain.WorkOrders.billing;

namespace MechanicShop.Application.Billing;

public static class InvoiceMapper
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        return new InvoiceDto
        {
            InvoiceId = invoice.Id,
            WorkOrderId = invoice.WorkOrderId,
            Customer = invoice.WorkOrder!.Vehicle!.Customer!.ToDto(),
            Vehicle = invoice.WorkOrder.Vehicle.ToDto(),
            IssuedAtUtc = invoice.IssuedAtUtc,
            Subtotal = invoice.Subtotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            Total = invoice.Total,
            PaymentStatus = invoice.Status.ToString(),
            Items = invoice.LineItems.Select(x => x.ToDto()).ToList()
        };
    }


    public static List<InvoiceDto> ToDtos(this IEnumerable<Invoice> entities) =>
        [.. entities.Select(e => e.ToDto())];


    public static InvoiceLineItemDto ToDto(this InvoiceLineItem item) => new()
    {
        InvoiceId = item.InvoiceId,
        LineNumber = item.LineNumber,
        Description = item.Description,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        LineTotal = item.LineTotal
    };


    public static List<InvoiceLineItemDto> ToDtos(this IEnumerable<InvoiceLineItem> entities) =>
        [.. entities.Select(e => e.ToDto())];
}
