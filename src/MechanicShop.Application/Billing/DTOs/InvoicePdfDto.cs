namespace MechanicShop.Application.Billing.DTOs;

public sealed class InvoicePdfDto
{
    public byte[]? Content { get; init; }
    public string? FileName { get; init; }
    public string? ContentType { get; init; } = "application/pdf";
}