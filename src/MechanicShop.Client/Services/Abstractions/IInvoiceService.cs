using MechanicShop.Client.Common;
using MechanicShop.Client.Models;

namespace MechanicShop.Client.Services.Abstractions;

public interface IInvoiceService
{
    Task<ApiResult<InvoiceModel>> IssueInvoiceAsync(Guid workorderId);

    Task<ApiResult<InvoiceModel>> GetInvoiceAsync(Guid invoiceId);

    Task<ApiResult<byte[]>> GetInvoicePdfAsync(Guid invoiceId);

    Task<ApiResult> SettleInvoice(Guid invoiceId);
}
