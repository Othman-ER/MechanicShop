using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.billing;

public sealed class InvoiceLineItem
{
    public Guid InvoiceId { get; }
    public int LineNumber { get; }
    public string? Description { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal LineTotal => Quantity * UnitPrice;


    private InvoiceLineItem() { }

    private InvoiceLineItem(
        Guid invoiceId,
        int lineNumber,
        string description,
        int quantity,
        decimal unitPrice)
    {
        InvoiceId = invoiceId;
        LineNumber = lineNumber;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    

    public static Result<InvoiceLineItem> Create(
        Guid invoiceId,
        int lineNumber,
        string description,
        int quantity,
        decimal unitPrice)
    {
        if (invoiceId == Guid.Empty)
            return InvoiceLineItemErrors.InvoiceIdRequired;

        if (lineNumber <= 0)
            return InvoiceLineItemErrors.LineNumberInvalid;

        if (string.IsNullOrWhiteSpace(description))
            return InvoiceLineItemErrors.DescriptionRequired;

        if (quantity <= 0)
            return InvoiceLineItemErrors.QuantityInvalid;

        if (unitPrice <= 0)
            return InvoiceLineItemErrors.UnitPriceInvalid;

        return new InvoiceLineItem(invoiceId, lineNumber, description.Trim(), quantity, unitPrice);
    }
}