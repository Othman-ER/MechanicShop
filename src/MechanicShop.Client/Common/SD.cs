using System.Net;
using System.Text.Json;
using MechanicShop.Client.Models;
using MechanicShop.Contracts.Requests.WorkOrders;

namespace MechanicShop.Client.Common;

/// <summary>Contains Static Methodes And Properties</summary>
public static class SD
{

    public static async Task<ApiResult<T>> HandleErrorResponseAsync<T>(HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();

        try
        {
            var problemDetails = JsonSerializer
                .Deserialize<ProblemDetails>(content, options: new() { PropertyNameCaseInsensitive = true });

            if (problemDetails is not null)
            {
                return ApiResult<T>.Failure(
                    message: problemDetails.Title ?? "An error occurred",
                    detail: problemDetails.Detail ?? "Error",
                    statusCode: problemDetails.Status ?? (int)response.StatusCode,
                    validationErrors: problemDetails.Errors);
            }

            return ApiResult<T>.Failure(
                message: GetFriendlyErrorMessage(response.StatusCode),
                detail: content,
                statusCode: (int)response.StatusCode);
        }
        catch (JsonException)
        {
            return ApiResult<T>.Failure(
                message: GetFriendlyErrorMessage(response.StatusCode),
                detail: content,
                statusCode: (int)response.StatusCode);
        }
    }

    public static Task<ApiResult> HandleErrorResponseAsync(HttpResponseMessage response) =>
        HandleErrorResponseAsync<object>(response)
            .ContinueWith(t =>
                ApiResult.Failure(
                    t.Result.ErrorMessage,
                    t.Result.ErrorDetail,
                    t.Result.StatusCode,
                    t.Result.ValidationErrors)
                );

    public static Task<ApiResult<T>> HandleExceptionAsync<T>(Exception ex, string message) =>
        Task.FromResult(ex switch
        {
            HttpRequestException => ApiResult<T>.Failure($"Network error occurred. {message}"),
            TaskCanceledException => ApiResult<T>.Failure($"Request timed out. {message}"),
            _ => ApiResult<T>.Failure($"An unexpected error occurred. {message}")
        });


    public static Task<ApiResult> HandleExceptionAsync(Exception ex, string message) =>
        HandleExceptionAsync<object>(ex, message).ContinueWith(t =>
            ApiResult.Failure(
                t.Result.ErrorMessage,
                t.Result.ErrorDetail,
                t.Result.StatusCode,
                t.Result.ValidationErrors)
            );


    public static string GetFriendlyErrorMessage(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "Invalid request. Please check your input and try again.",
        HttpStatusCode.Unauthorized => "You are not authorized to perform this action.",
        HttpStatusCode.Forbidden => "You don't have permission to perform this action.",
        HttpStatusCode.NotFound => "The requested resource was not found.",
        HttpStatusCode.Conflict => "The operation conflicts with the current state of the resource.",
        HttpStatusCode.UnprocessableEntity => "The request contains invalid data.",
        HttpStatusCode.InternalServerError => "A server error occurred. Please try again later.",
        HttpStatusCode.BadGateway => "Service temporarily unavailable. Please try again later.",
        HttpStatusCode.ServiceUnavailable => "Service temporarily unavailable. Please try again later.",
        HttpStatusCode.GatewayTimeout => "The request timed out. Please try again.",
        _ => "An error occurred while processing your request."
    };


    public static string BuildQueryString(WorkOrderFilterRequest filterRequest, PageRequest pageRequest)
    {
        var queryParams = new List<string>
        {
            $"page={pageRequest.Page}",
            $"pageSize={pageRequest.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(filterRequest.SearchTerm))
            queryParams.Add($"searchTerm={Uri.EscapeDataString(filterRequest.SearchTerm)}");

        if (!string.IsNullOrWhiteSpace(filterRequest.SortColumn))
            queryParams.Add($"sortColumn={Uri.EscapeDataString(filterRequest.SortColumn)}");

        if (!string.IsNullOrWhiteSpace(filterRequest.SortDirection))
            queryParams.Add($"sortDirection={Uri.EscapeDataString(filterRequest.SortDirection)}");

        if (filterRequest.State.HasValue)
            queryParams.Add($"state={filterRequest.State}");

        if (filterRequest.VehicleId.HasValue && filterRequest.VehicleId != Guid.Empty)
            queryParams.Add($"vehicleId={filterRequest.VehicleId}");

        if (filterRequest.LaborId.HasValue && filterRequest.LaborId != Guid.Empty)
            queryParams.Add($"laborId={filterRequest.LaborId}");

        if (filterRequest.StartDateFrom.HasValue)
            queryParams.Add($"startDateFrom={filterRequest.StartDateFrom:yyyy-MM-ddTHH:mm:ss}");

        if (filterRequest.StartDateTo.HasValue)
            queryParams.Add($"startDateTo={filterRequest.StartDateTo:yyyy-MM-ddTHH:mm:ss}");

        if (filterRequest.EndDateFrom.HasValue)
            queryParams.Add($"endDateFrom={filterRequest.EndDateFrom:yyyy-MM-ddTHH:mm:ss}");

        if (filterRequest.EndDateTo.HasValue)
            queryParams.Add($"endDateTo={filterRequest.EndDateTo:yyyy-MM-ddTHH:mm:ss}");

        if (filterRequest.Spot.HasValue)
            queryParams.Add($"spot={filterRequest.Spot}");

        return string.Join("&", queryParams);
    }

    public static Dictionary<string, List<string>> CarMakes => new()
    {
        ["Acura"] = ["ILX", "MDX", "NSX", "RDX", "RLX", "TLX", "TSX", "ZDX"],
        ["Audi"] = ["A3", "A4", "A5", "A6", "A7", "A8", "Q3", "Q5", "Q7", "Q8", "R8", "RS3", "RS4", "RS5", "RS6", "RS7", "S3", "S4", "S5", "S6", "S7", "S8", "SQ5", "SQ7", "SQ8", "TT", "e-tron"],
        ["BMW"] = ["1 Series", "2 Series", "3 Series", "4 Series", "5 Series", "6 Series", "7 Series", "8 Series", "X1", "X2", "X3", "X4", "X5", "X6", "X7", "Z4", "i3", "i4", "i8", "iX"],
        ["Buick"] = ["Enclave", "Encore", "Envision", "LaCrosse", "Regal", "Verano"],
        ["Cadillac"] = ["ATS", "CT4", "CT5", "CTS", "DTS", "Escalade", "SRX", "STS", "XT4", "XT5", "XT6", "XTS"],
        ["Chevrolet"] = ["Blazer", "Bolt", "Camaro", "Colorado", "Corvette", "Cruze", "Equinox", "Express", "Impala", "Malibu", "Silverado", "Sonic", "Spark", "Suburban", "Tahoe", "Traverse", "Trax", "Volt"],
        ["Chrysler"] = ["200", "300", "Pacifica", "Town & Country", "Voyager"],
        ["Dodge"] = ["Challenger", "Charger", "Dart", "Durango", "Grand Caravan", "Journey", "Ram 1500", "Ram 2500", "Ram 3500"],
        ["Ford"] = ["Bronco", "EcoSport", "Edge", "Escape", "Expedition", "Explorer", "F-150", "F-250", "F-350", "Fiesta", "Flex", "Focus", "Fusion", "Mustang", "Ranger", "Super Duty", "Taurus", "Transit"],
        ["Genesis"] = ["G70", "G80", "G90", "GV60", "GV70", "GV80"],
        ["GMC"] = ["Acadia", "Canyon", "Sierra", "Terrain", "Yukon"],
        ["Honda"] = ["Accord", "Civic", "CR-V", "CR-Z", "Fit", "HR-V", "Insight", "Odyssey", "Passport", "Pilot", "Ridgeline"],
        ["Hyundai"] = ["Accent", "Azera", "Elantra", "Genesis", "Ioniq", "Kona", "Nexo", "Palisade", "Santa Fe", "Sonata", "Tucson", "Veloster", "Venue"],
        ["Infiniti"] = ["Q50", "Q60", "Q70", "QX30", "QX50", "QX60", "QX70", "QX80"],
        ["Jaguar"] = ["E-PACE", "F-PACE", "F-TYPE", "I-PACE", "XE", "XF", "XJ"],
        ["Jeep"] = ["Cherokee", "Compass", "Gladiator", "Grand Cherokee", "Patriot", "Renegade", "Wrangler"],
        ["Kia"] = ["Cadenza", "Forte", "K5", "Niro", "Optima", "Rio", "Sedona", "Seltos", "Sorento", "Soul", "Sportage", "Stinger", "Telluride"],
        ["Land Rover"] = ["Defender", "Discovery", "Discovery Sport", "Range Rover", "Range Rover Evoque", "Range Rover Sport", "Range Rover Velar"],
        ["Lexus"] = ["CT", "ES", "GX", "IS", "LC", "LS", "LX", "NX", "RC", "RX", "UX"],
        ["Lincoln"] = ["Aviator", "Continental", "Corsair", "MKC", "MKT", "MKX", "MKZ", "Navigator", "Nautilus"],
        ["Mazda"] = ["CX-3", "CX-30", "CX-5", "CX-9", "Mazda3", "Mazda6", "MX-5 Miata"],
        ["Mercedes-Benz"] = ["A-Class", "C-Class", "CLA", "CLS", "E-Class", "G-Class", "GLA", "GLB", "GLC", "GLE", "GLS", "S-Class", "SL", "SLC"],
        ["Mitsubishi"] = ["Eclipse Cross", "Lancer", "Mirage", "Outlander", "Outlander Sport"],
        ["Nissan"] = ["370Z", "Altima", "Armada", "Frontier", "GT-R", "Kicks", "Leaf", "Maxima", "Murano", "NV200", "Pathfinder", "Rogue", "Sentra", "Titan", "Versa"],
        ["Porsche"] = ["718 Boxster", "718 Cayman", "911", "Cayenne", "Macan", "Panamera", "Taycan"],
        ["Ram"] = ["1500", "2500", "3500", "ProMaster", "ProMaster City"],
        ["Subaru"] = ["Ascent", "BRZ", "Crosstrek", "Forester", "Impreza", "Legacy", "Outback", "WRX"],
        ["Tesla"] = ["Model 3", "Model S", "Model X", "Model Y", "Cybertruck", "Roadster"],
        ["Toyota"] = ["4Runner", "86", "Avalon", "C-HR", "Camry", "Corolla", "Highlander", "Land Cruiser", "Prius", "RAV4", "Sequoia", "Sienna", "Supra", "Tacoma", "Tundra", "Venza", "Yaris"],
        ["Volkswagen"] = ["Arteon", "Atlas", "Beetle", "Golf", "Jetta", "Passat", "Tiguan", "Touareg"],
        ["Volvo"] = ["S60", "S90", "V60", "V90", "XC40", "XC60", "XC90"]
    };

}
