using MechanicShop.Client.Common;
using MechanicShop.Contracts.Responses;

namespace MechanicShop.Client.Services.Abstractions;

public interface ISettingService
{
    Task<ApiResult<OperatingHoursResponse>> GetOperateHoursAsync();
}
