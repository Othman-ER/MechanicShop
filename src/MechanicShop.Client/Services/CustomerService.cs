using System.Net.Http.Json;
using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Requests.Customers;

namespace MechanicShop.Client.Services;

public class CustomerService(IHttpClientFactory httpClientFactory) : ICustomerService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MechanicShopApi");

    public async Task<ApiResult<List<CustomerModel>>> GetCustomersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/customers");

            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadFromJsonAsync<List<CustomerModel>>();
                return ApiResult<List<CustomerModel>>.Success(customers ?? []);
            }

            return await SD.HandleErrorResponseAsync<List<CustomerModel>>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<List<CustomerModel>>(ex, "Failed to retrieve customers");
        }
    }

    public async Task<ApiResult<CustomerModel>> GetByIdAsync(Guid customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/customers/{customerId}");

            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<CustomerModel>();

                return ApiResult<CustomerModel>.Success(customer!);
            }

            return await SD.HandleErrorResponseAsync<CustomerModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<CustomerModel>(ex, $"Failed to retrieve customer {customerId}");
        }
    }


    public async Task<ApiResult<CustomerModel>> CreateAsync(CreateCustomerRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/customers", request);

            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<CustomerModel>();

                if (customer is null)
                {
                    return ApiResult<CustomerModel>.Failure("Customer response was null.");
                }

                return ApiResult<CustomerModel>.Success(customer);
            }

            return await SD.HandleErrorResponseAsync<CustomerModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<CustomerModel>(ex, "Failed to create customer.");
        }
    }

    public async Task<ApiResult<CustomerModel>> UpdateAsync(Guid customerId, UpdateCustomerRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/v1/customers/{customerId}", request);

            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<CustomerModel>();
                return ApiResult<CustomerModel>.Success(customer!);
            }

            return await SD.HandleErrorResponseAsync<CustomerModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<CustomerModel>(ex, $"Failed to update customer {customerId}");
        }
    }

    public async Task<ApiResult> DeleteByIdAsync(Guid customerId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/v1/customers/{customerId}");

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to delete customer {customerId}");
        }
    }

    public async Task<ApiResult<List<VehicleModel>>> GetVehiclesByCustomerIdAsync(Guid customerId)
    {
        try
        {
            var customerResult = await GetByIdAsync(customerId);

            if (!customerResult.IsSuccess)
            {
                return ApiResult<List<VehicleModel>>.Failure(
                    customerResult.ErrorMessage,
                    customerResult.ErrorDetail,
                    customerResult.StatusCode,
                    customerResult.ValidationErrors
                );
            }

            var vehicles = customerResult.Data?.Vehicles ?? [];
            return ApiResult<List<VehicleModel>>.Success(vehicles);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<List<VehicleModel>>
                (ex, $"Failed to retrieve vehicles for customer {customerId}");
        }
    }
}
