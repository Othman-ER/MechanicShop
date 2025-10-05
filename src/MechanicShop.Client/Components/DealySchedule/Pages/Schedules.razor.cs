using System.Security.Claims;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace MechanicShop.Client.Components.DealySchedule.Pages;

public partial class Schedules
{

    [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;

    [Inject] private IScheduleService ScheduleService { get; set; } = default!;
    [Inject] private ISettingService SettingsService { get; set; } = default!;
    [Inject] private ILaborService LaborService { get; set; } = default!;
    [Inject] private IWorkOrderService WorkOrderService { get; set; } = default!;

    [Inject] private NavigationManager _navigation { get; set; } = default!;
    [Inject] private IJSRuntime _jsRuntime { get; set; } = default!;

    [Parameter] public DateTime? SelectedDate { get; set; }
    private DateTime _selectedDate;

    private ScheduleModel _schedule = new();
    private List<LaborModel>? _labors;
    private Guid? _selectedLaborId;
    private AvailabilitySlotModel? SelectedSlot;
    private TimeSpan _maxAllowedDuration;
    private RenderFragment? _activeDialogContent;

    private bool _isPM;
    private string? _userId;

    private bool _isLoading;
    private bool _showDialog;


    private RenderFragment? _dialogTitle;
    private RenderFragment? _dialogMeta;

    private CancellationTokenSource? _cancellationTokenSource;

    private OperatingHoursResponse operateHours = default!;

    protected override async Task OnInitializedAsync()
    {
        _cancellationTokenSource = new();

        _ = StartRefreshing(_cancellationTokenSource.Token);

        _selectedDate = SelectedDate ?? DateTime.Today;

        var openningHoursResult = await SettingsService.GetOperateHoursAsync();

        if (openningHoursResult.IsSuccess)
            operateHours = openningHoursResult.Data!;

        var laborResult = await LaborService.GetLaborsAsync();

        if (laborResult.IsSuccess)
            _labors = laborResult.Data!;

        var user = (await _authStateProvider.GetAuthenticationStateAsync()).User;

        _userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        _isPM = user.IsInRole("Manager");

        await UpdateDaySchedule();
    }


    private async Task StartRefreshing(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            StateHasChanged();

            await Task.Delay(30000, token);
        }
    }


    private async Task UpdateDaySchedule()
    {
        _isLoading = true;

        StateHasChanged();

        var scheduleResult = await ScheduleService.GetDailyScheduleAsync(
            DateOnly.FromDateTime(_selectedDate),
            _selectedLaborId
        );

        if (scheduleResult.IsSuccess)
            _schedule = scheduleResult.Data!;

        _isLoading = false;

        await ScrollToBottom();
        StateHasChanged();

    }


    private async Task GoToPreviousDay()
    {
        _selectedDate = _selectedDate.AddDays(-1);

        NavigateToCurrentDate();
        await UpdateDaySchedule();
    }


    private async Task GoToNextDay()
    {
        _selectedDate = _selectedDate.AddDays(1);

        NavigateToCurrentDate();
        await UpdateDaySchedule();
    }


    private void NavigateToCurrentDate()
    {
        var dateParam = _selectedDate.ToString("yyyy-MM-dd");
        _navigation.NavigateTo($"/schedules/{dateParam}");
    }

    private async Task OnDateChanged(ChangeEventArgs _) => await UpdateDaySchedule();


    private async Task DeleteWorkOrder(Guid workOrderId)
    {
        var deleteWorkOrderResult = await WorkOrderService.DeleteByIdAsync(workOrderId);

        if (deleteWorkOrderResult.IsSuccess)
            await UpdateDaySchedule();
    }

    private async Task OnWorkOrderCreatedHandler()
    {
        HideDialog();

        await UpdateDaySchedule();
    }

    private void HideDialog()
    {
        _showDialog = false;
        _activeDialogContent = null;
        SelectedSlot = null;
    }

    private void ShowDialog(RenderFragment content)
    {
        _activeDialogContent = content;
        _showDialog = true;
    }

    private IEnumerable<TimeOnly> GenerateTimeSlots()
    {
        var start = operateHours.OpeningTime;
        var end = operateHours.ClosingTime;

        var maxIterations = 24 * 60 / 15;

        var current = start;
        var iteration = 0;

        while (iteration++ < maxIterations)
        {
            yield return current;
            current = current.Add(TimeSpan.FromMinutes(15));

            if (current >= end)
                break;

            if (current == start)
                break;
        }
    }

    private void ShowNewWorkOrderDialog(AvailabilitySlotModel slot)
    {
        SelectedSlot = slot;
        _maxAllowedDuration = slot.EndAt - slot.StartAt;

        _dialogTitle = builder => builder.AddContent(0, $"New WorkOrder");

        _dialogMeta = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "bg-body-tertiary text-light border border-secondary rounded-2 px-3 py-2 mb-3 small d-flex flex-wrap gap-4 align-items-center");

            builder.AddMarkupContent(2, $"<span>⏰ <strong>Time:</strong> {slot.StartAt:HH\\:mm} ➔ {slot.EndAt:HH\\:mm}</span>");

            builder.CloseElement();
        };


        ShowDialog(builder =>
        {
            builder.OpenComponent<WorkOrderForm>(0);
            builder.AddAttribute(1, "MaxAllowedDuration", _maxAllowedDuration);
            builder.AddAttribute(2, "OnWorkOrderCreated", EventCallback.Factory.Create(this, OnWorkOrderCreatedHandler));
            builder.AddAttribute(3, "StartAt", slot.StartAt.LocalDateTime);
            builder.AddAttribute(4, "Spot", slot.Spot);
            builder.AddAttribute(5, "Disabled", slot.WorkOrderLocked);
            builder.CloseComponent();
        });
    }

    private void ShowEditLaborDialog(AvailabilitySlotModel slot)
    {
        _dialogTitle = builder => builder.AddContent(0, $"Reassign Labor");

        _dialogMeta = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "bg-body-tertiary text-light border border-secondary rounded-2 px-3 py-2 mb-3 small d-flex flex-wrap gap-4 align-items-center");

            builder.AddMarkupContent(2, $"<span>🏷️ <strong>Workorder:</strong> {slot.WorkOrderId.ToString()?[..8]}</span>");
            builder.AddMarkupContent(2, $"<span>⏰ <strong>Time:</strong> {slot.StartAt:HH\\:mm} ➔ {slot.EndAt:HH\\:mm}</span>");
            builder.AddMarkupContent(3, $"<span>�� <strong>Vehicle:</strong> {slot.Vehicle}</span>");
            builder.AddMarkupContent(4, $"<span>👷 <strong>Labor:</strong> {slot.Labor?.Name}</span>");

            builder.CloseElement();
        };

        ShowDialog(builder =>
        {
            builder.OpenComponent<EditWorkOrderLaborDialog>(0);
            builder.AddAttribute(1, "WorkOrderId", slot.WorkOrderId);
            builder.AddAttribute(2, "SelectedLabor", slot.Labor);
            builder.AddAttribute(3, "OnChange", EventCallback.Factory.Create(this, OnWorkOrderChanged));
            builder.CloseComponent();
        });
    }

    private void ShowEditRepairTasksDialog(AvailabilitySlotModel slot)
    {
        _dialogTitle = builder => builder.AddContent(0, $"Modify Workorder Tasks");

        _dialogMeta = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "bg-body-tertiary text-light border border-secondary rounded-2 px-3 py-2 mb-3 small d-flex flex-wrap gap-4 align-items-center");

            builder.AddMarkupContent(2, $"<span>🏷️ <strong>Workorder:</strong> {slot.WorkOrderId.ToString()?[..8]}</span>");
            builder.AddMarkupContent(2, $"<span>⏰ <strong>Time:</strong> {slot.StartAt:HH\\:mm} ➔ {slot.EndAt:HH\\:mm}</span>");
            builder.AddMarkupContent(3, $"<span>🚗 <strong>Vehicle:</strong> {slot.Vehicle}</span>");
            builder.AddMarkupContent(4, $"<span>👷 <strong>Labor:</strong> {slot.Labor?.Name}</span>");

            builder.CloseElement();
        };

        ShowDialog(builder =>
        {
            builder.OpenComponent<EditWorkOrderRepairTaskDialog>(0);
            builder.AddAttribute(1, "WorkOrderId", slot.WorkOrderId);
            builder.AddAttribute(2, "SelectedRepairTaskIds", slot.RepairTasks.Select(x => x.RepairTaskId).ToList());
            builder.AddAttribute(3, "OnChange", EventCallback.Factory.Create(this, OnWorkOrderChanged));
            builder.CloseComponent();
        });
    }

    private void ShowEditStateDialog(AvailabilitySlotModel slot)
    {
        _dialogTitle = builder => builder.AddContent(0, $"Update Workorder State");

        _dialogMeta = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "bg-body-tertiary text-light border border-secondary rounded-2 px-3 py-2 mb-3 small d-flex flex-wrap gap-4 align-items-center");

            builder.AddMarkupContent(2, $"<span>🏷️ <strong>Workorder:</strong> {slot.WorkOrderId.ToString()?.Substring(0, 8)}</span>");
            builder.AddMarkupContent(2, $"<span>⏰ <strong>Time:</strong> {slot.StartAt:HH\\:mm} ➔ {slot.EndAt:HH\\:mm}</span>");
            builder.AddMarkupContent(3, $"<span>🚗 <strong>Vehicle:</strong> {slot.Vehicle}</span>");
            builder.AddMarkupContent(4, $"<span>👷 <strong>Labor:</strong> {slot.Labor?.Name}</span>");

            builder.CloseElement();
        };

        ShowDialog(builder =>
        {
            builder.OpenComponent<EditWorkOrderStateDialog>(0);
            builder.AddAttribute(1, "WorkOrderState", slot.State);
            builder.AddAttribute(2, "WorkOrderId", slot.WorkOrderId);
            builder.AddAttribute(3, "IsReadonlyDueToOwnership", !_isPM && slot.Labor?.LaborId != _userId);
            builder.AddAttribute(4, "OnChange", EventCallback.Factory.Create(this, OnWorkOrderChanged));
            builder.CloseComponent();
        });
    }


    private async Task OnWorkOrderChanged()
    {
        HideDialog();
        await UpdateDaySchedule();
    }


    private void ShowRelocateWorkOrderDialog(AvailabilitySlotModel slot)
    {
        var currentSpot = _schedule.Spots.First(s => s.Spot == slot.Spot);
        var currentIndex = currentSpot.Slots.IndexOf(slot);

        var nextFreeSlot = currentSpot.Slots
            .Skip(currentIndex + 1)
            .FirstOrDefault(s => !s.IsOccupied);

        var maxEndTime = nextFreeSlot?.EndAt;
        var maxAllowedDuration = maxEndTime - slot.StartAt;

        _dialogTitle = builder => builder.AddContent(0, $"Relocate Workorder");

        _dialogMeta = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "bg-body-tertiary text-light border border-secondary rounded-2 px-3 py-2 mb-3 small d-flex flex-wrap gap-4 align-items-center");

            builder.AddMarkupContent(2, $"<span>🏷️ <strong>Workorder:</strong> {slot.WorkOrderId.ToString()?.Substring(0, 8)}</span>");
            builder.AddMarkupContent(2, $"<span>⏰ <strong>Time:</strong> {slot.StartAt:HH\\:mm} ➔ {slot.EndAt:HH\\:mm}</span>");
            builder.AddMarkupContent(3, $"<span>🚗 <strong>Vehicle:</strong> {slot.Vehicle}</span>");
            builder.AddMarkupContent(4, $"<span>👷 <strong>Labor:</strong> {slot.Labor?.Name}</span>");

            builder.CloseElement();
        };

        ShowDialog(builder =>
        {
            builder.OpenComponent<RelocateWorkOrderDialog>(0);
            builder.AddAttribute(1, "WorkOrderId", slot.WorkOrderId);
            builder.AddAttribute(2, "StartAt", slot.StartAt);
            builder.AddAttribute(3, "CurrentSpot", slot.Spot);
            builder.AddAttribute(4, "OnChange", EventCallback.Factory.Create(this, OnWorkOrderChanged));
            builder.CloseComponent();
        });
    }


    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }


    private double GetNowLinePosition()
    {
        if (operateHours is null) return -1;
        if (_selectedDate.Date != DateTime.Today) return -1;

        var now = TimeOnly.FromDateTime(DateTime.Now);
        var open = operateHours.OpeningTime;
        var close = operateHours.ClosingTime;

        if (open >= close) return -1;
        if (now < open || now > close) return -1;

        const double pixelsPer15Min = 62.0;

        var totalMinutes = (close - open).TotalMinutes;
        var totalPixels = totalMinutes / 15.0 * pixelsPer15Min;

        var minutesFromStart = (now - open).TotalMinutes;
        var offsetPixels = minutesFromStart / 15.0 * pixelsPer15Min;

        var percentage = offsetPixels / totalPixels * 100.0;

        return Math.Round(percentage, 2);
    }


    private async Task ScrollToBottom() => await _jsRuntime.InvokeVoidAsync("scrollToElement", "nowLineTarget");
}