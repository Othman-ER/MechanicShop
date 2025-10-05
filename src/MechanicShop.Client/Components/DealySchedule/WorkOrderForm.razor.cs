using MechanicShop.Client.Identity;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Common;
using MechanicShop.Contracts.Requests.WorkOrders;
using Microsoft.AspNetCore.Components;

namespace MechanicShop.Client.Components.DealySchedule;

public partial class WorkOrderForm
{
    [Inject] private ICustomerService CustomerService { get; set; } = default!;
    [Inject] private ILaborService LaborService { get; set; } = default!;
    [Inject] private IRepairTaskservice RepairTaskservice { get; set; } = default!;
    [Inject] private IWorkOrderService WorkOrderService { get; set; } = default!;

    [Inject] private IAccountManagement AccountManagement { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public EventCallback OnWorkOrderCreated { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public DateTime StartAt { get; set; } = default!;

    [Parameter]
    public Spot Spot { get; set; }

    [Parameter]
    public TimeSpan MaxAllowedDuration { get; set; } = TimeSpan.FromHours(8);

    [Parameter]
    public bool Disabled { get; set; }

    private DateTime _localStartDate;
    private TimeSpan _localStartTime;

    private DateTime LocalStartAt
    {
        get => _localStartDate.Date + _localStartTime;
        set
        {
            _localStartDate = value.Date;
            _localStartTime = TimeSpan.FromMinutes(Math.Round(value.TimeOfDay.TotalMinutes / 15.0) * 15);
            var local = _localStartDate + _localStartTime;
            var offset = TimeZoneInfo.Local.GetUtcOffset(local);
            createWorkOrderRequest.StartAtUtc = new DateTimeOffset(local, offset);
        }
    }

    private CreateWorkOrderRequest createWorkOrderRequest = new();

    private List<CustomerModel>? customers = [];
    private List<VehicleModel>? vehicles = [];
    private List<RepairTaskModel>? repairServices = [];
    private List<LaborModel>? labors = [];

    private bool isLoading = true;

    private Guid? selectedCustomerId;

    private string? errorMessage;



    protected override async Task OnInitializedAsync()
    {
        isLoading = true;

        var getCustomersTask = CustomerService.GetCustomersAsync();
        var getRepairTasksTask = RepairTaskservice.GetRepairTasksAsync();
        var getLaborsTask = LaborService.GetLaborsAsync();

        await Task.WhenAll(getCustomersTask, getRepairTasksTask, getLaborsTask);

        var customersResult = getCustomersTask.Result;
        var repairResult = getRepairTasksTask.Result;
        var laborsResult = getLaborsTask.Result;

        if (customersResult.IsSuccess)
            customers = customersResult.Data;

        if (repairResult.IsSuccess)

            repairServices = repairResult.Data;

        if (laborsResult.IsSuccess)
            labors = laborsResult.Data;

        createWorkOrderRequest.RepairTaskIds = [];

        isLoading = false;
    }


    protected override void OnParametersSet()
    {
        LocalStartAt = StartAt;
        createWorkOrderRequest.Spot = Spot;
    }


    private async Task LoadVehicles()
    {
        if (selectedCustomerId.HasValue)
        {
            var customerVehicleResult = await CustomerService.GetVehiclesByCustomerIdAsync(selectedCustomerId.Value);

            if (customerVehicleResult.IsSuccess)
                vehicles = customerVehicleResult.Data;
        }
        else
        {
            vehicles = [];
        }
    }

    private List<TimeSpan> TimeOptions => [.. Enumerable.Range(0, 96).Select(i => TimeSpan.FromMinutes(i * 15))];


    private async Task HandleSubmit()
    {
        var local = _localStartDate + _localStartTime;
        var offset = TimeZoneInfo.Local.GetUtcOffset(local);
        createWorkOrderRequest.StartAtUtc = new DateTimeOffset(local, offset);

        var createOrderResult = await WorkOrderService.CreateAsync(createWorkOrderRequest);

        if (createOrderResult.IsSuccess)
            await OnWorkOrderCreated.InvokeAsync();
        else
            errorMessage = createOrderResult.FirstErrorMessage;
    }


    private void OnTaskAdded(RepairTaskModel repairTask)
    {
        if (!createWorkOrderRequest.RepairTaskIds.Contains(repairTask.RepairTaskId))
            createWorkOrderRequest.RepairTaskIds.Add(repairTask.RepairTaskId);
    }


    private void OnTaskRemoved(Guid repairTaskId) => createWorkOrderRequest.RepairTaskIds.Remove(repairTaskId);


    private async Task Cancel() => await OnWorkOrderCreated.InvokeAsync();
}