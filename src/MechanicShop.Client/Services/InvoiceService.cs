using System.Net.Http.Json;
using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;

namespace MechanicShop.Client.Services;

public class InvoiceService(IHttpClientFactory httpClientFactory) : IInvoiceService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MechanicShopApi");


    public async Task<ApiResult<InvoiceModel>> IssueInvoiceAsync(Guid workorderId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/v1/invoices/workorders/{workorderId}", null);

            if (response.IsSuccessStatusCode)
            {
                var invoice = await response.Content.ReadFromJsonAsync<InvoiceModel>();

                if (invoice == null)
                {
                    return ApiResult<InvoiceModel>.Failure("Invoice response was null");
                }

                return ApiResult<InvoiceModel>.Success(invoice);
            }

            return await SD.HandleErrorResponseAsync<InvoiceModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<InvoiceModel>
                (ex, $"Failed to issue invoice for work order {workorderId}");
        }
    }


    public async Task<ApiResult<InvoiceModel>> GetInvoiceAsync(Guid invoiceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/invoices/{invoiceId}");

            if (response.IsSuccessStatusCode)
            {
                var invoice = await response.Content.ReadFromJsonAsync<InvoiceModel>();
                return ApiResult<InvoiceModel>.Success(invoice!);
            }

            return await SD.HandleErrorResponseAsync<InvoiceModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<InvoiceModel>
                (ex, $"Failed to retrieve invoice {invoiceId}");
        }
    }

    public async Task<ApiResult<byte[]>> GetInvoicePdfAsync(Guid invoiceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/invoices/{invoiceId}/pdf");

            if (response.IsSuccessStatusCode)
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                return ApiResult<byte[]>.Success(pdfBytes);
            }

            return await SD.HandleErrorResponseAsync<byte[]>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<byte[]>
                (ex, $"Failed to retrieve PDF for invoice {invoiceId}");
        }
    }

    public async Task<ApiResult> SettleInvoice(Guid invoiceId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/v1/invoices/{invoiceId}/payments", null);

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to settle invoice {invoiceId}");
        }
    }
}
