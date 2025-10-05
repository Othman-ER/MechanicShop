using MechanicShop.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MechanicShop.Client.Components.DealySchedule;

public partial class AvailabilitySlotComponent
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public AvailabilitySlotModel Slot { get; set; } = default!;

    [Parameter, EditorRequired]
    public EventCallback<AvailabilitySlotModel> OnNewWorkOrderDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public EventCallback<AvailabilitySlotModel> OnEditWorkOrderLaborDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public EventCallback<AvailabilitySlotModel> OnRelocateWorkOrderDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public EventCallback<AvailabilitySlotModel> OnEditWorkOrderRepairTasksDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public EventCallback<AvailabilitySlotModel> OnEditWorkOrderStateDialog { get; set; } = default!;

    [Parameter] public EventCallback<Guid> OnDeleteWorkOrder { get; set; } = default!;


    public string GetDynamicClasses()
    {
        var baseClasses = "booking-container w-100 fw-semibold bg-opacity-25 ";

        baseClasses += Slot.IsOccupied ? "bg-secondary" : "bg-success";

        if (!Slot.IsOccupied && !Slot.IsAvailable)
            baseClasses += " unavailable";

        return baseClasses;
    }


    public string GetDynamicStyle()
    {
        var duration = (Slot.EndAt - Slot.StartAt).TotalMinutes;
        var height = duration * 4;

        return $"height: {height}px; outline: 2px solid #212529; outline-offset: -1px; padding:0!important;";
    }


    private async Task ConfirmDeleteAsync(AvailabilitySlotModel slot)
    {
        if (slot.WorkOrderId is not null)
        {
            var message = $"Delete WorkOrder?\n\n" +
            $"⏰ Time: {slot.StartAt:hh\\:mm} ➔ {slot.EndAt:hh\\:mm}\n" +
            $"🚗 Vehicle: {slot.Vehicle}\n" +
            $"👷 Labor: {slot.Labor!.Name}\n";

            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", message);

            if (confirmed)
                await OnDeleteWorkOrder.InvokeAsync(slot.WorkOrderId.Value);
        }
    }
}