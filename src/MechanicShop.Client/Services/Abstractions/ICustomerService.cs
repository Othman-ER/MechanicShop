using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Contracts.Requests.Customers;

namespace MechanicShop.Client.Services.Abstractions;

public interface ICustomerService
{
    Task<ApiResult<List<CustomerModel>>> GetCustomersAsync();

    Task<ApiResult<CustomerModel>> GetByIdAsync(Guid customerId);

    Task<ApiResult<CustomerModel>> CreateAsync(CreateCustomerRequest request);

    Task<ApiResult<CustomerModel>> UpdateAsync(Guid customerId, UpdateCustomerRequest request);

    Task<ApiResult> DeleteByIdAsync(Guid customerId);

    Task<ApiResult<List<VehicleModel>>> GetVehiclesByCustomerIdAsync(Guid customerId);
}
