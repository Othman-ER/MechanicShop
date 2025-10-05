using MechanicShop.Client.Common;
using MechanicShop.Client.Models;

namespace MechanicShop.Client.Services.Abstractions;

public interface ILaborService
{
    Task<ApiResult<List<LaborModel>>> GetLaborsAsync();
}
