using System.Globalization;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Requests.RepairTasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MechanicShop.Client.Components.ReparTasks;

public partial class RepairTaskList
{
    [Inject] private IRepairTaskservice RepairTaskService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<RepairTaskModel> repairTasks = [];
    private List<RepairTaskModel> filteredTasks = [];
    private CreateRepairTaskRequest newTaskRequest = new();
    private UpdateRepairTaskRequest updateTaskRequest = new();
    private bool isEditMode = false;
    private Guid editingTaskId;
    private string searchTerm = "";
    private string sortBy = "name";
    private bool sortAscending = true;

    private bool _showDialog = false;
    private RenderFragment? _dialogTitle;

    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadRepairTasks();
        ApplyFilters();
    }

    private async Task LoadRepairTasks()
    {
        var repairTaskResult = await RepairTaskService.GetRepairTasksAsync();
        if (repairTaskResult.IsSuccess)
        {
            repairTasks = repairTaskResult.Data!;
        }
    }

    private void OnSearchChanged(ChangeEventArgs e)
    {
        searchTerm = e.Value?.ToString() ?? "";
        ApplyFilters();
    }

    private void SetSortDirection(bool ascending)
    {
        sortAscending = ascending;
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = repairTasks.AsEnumerable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            filtered = filtered.Where(t =>
            t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            t.Parts.Any(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        filtered = sortBy switch
        {
            "cost" => sortAscending ? filtered.OrderBy(t => t.TotalCost) : filtered.OrderByDescending(t => t.TotalCost),
            "duration" => sortAscending ? filtered.OrderBy(t => t.EstimatedDurationInMins) : filtered.OrderByDescending(t =>
            t.EstimatedDurationInMins),
            "labor" => sortAscending ? filtered.OrderBy(t => t.LaborCost) : filtered.OrderByDescending(t => t.LaborCost),
            _ => sortAscending ? filtered.OrderBy(t => t.Name) : filtered.OrderByDescending(t => t.Name)
        };

        filteredTasks = filtered.ToList();
    }

    private void ShowCreateModal()
    {
        isEditMode = false;
        newTaskRequest = new CreateRepairTaskRequest();
        AddPart();
        _dialogTitle = builder => builder.AddContent(0, "Create New Repair Task");
        _showDialog = true;
        errorMessage = null;
    }

    private void ShowEditModal(RepairTaskModel task)
    {
        isEditMode = true;
        editingTaskId = task.RepairTaskId;
        updateTaskRequest = new UpdateRepairTaskRequest
        {
            Name = task.Name,
            LaborCost = task.LaborCost,
            EstimatedDurationInMins = task.EstimatedDurationInMins,
            Parts = [.. task.Parts.Select(p => new UpdateRepairTaskPartRequest
            {
                PartId = p.PartId,
                Name = p.Name,
                Cost = p.Cost,
                Quantity = p.Quantity
            })]
        };
        _dialogTitle = builder => builder.AddContent(0, "Edit Repair Task");
        _showDialog = true;
        errorMessage = null;
    }

    private void HideDialog()
    {
        _showDialog = false;
        errorMessage = null;
    }


    private void AddPart()
    {
        if (!isEditMode)
            newTaskRequest.Parts.Add(new CreateRepairTaskPartRequest { Name = "", Cost = 0, Quantity = 1 });
        else
            updateTaskRequest.Parts.Add(new UpdateRepairTaskPartRequest { Name = "", Cost = 0, Quantity = 1 });
    }


    private void RemovePart(int index)
    {
        if (newTaskRequest.Parts.Count > 1)
            newTaskRequest.Parts.RemoveAt(index);
    }

    private void RemovePartFromUpdate(int index)
    {
        if (updateTaskRequest.Parts.Count > 1)
            updateTaskRequest.Parts.RemoveAt(index);
    }

    private async Task SaveTask()
    {
        errorMessage = null;

        if (isEditMode)
        {
            var task = repairTasks.FirstOrDefault(t => t.RepairTaskId == editingTaskId);
            if (task != null)
            {
                var updateTaskResult = await RepairTaskService.UpdateAsync(task.RepairTaskId, updateTaskRequest);

                if (updateTaskResult.IsSuccess)
                {
                    HideDialog();
                    await LoadRepairTasks();
                    ApplyFilters();
                }
                else
                {
                    errorMessage = updateTaskResult.FirstErrorMessage;
                }
            }
        }
        else
        {
            var createTaskResult = await RepairTaskService.CreateAsync(newTaskRequest);

            if (createTaskResult.IsSuccess)
            {
                HideDialog();
                repairTasks.Add(createTaskResult.Data!);
                ApplyFilters();
            }
            else
            {
                errorMessage = createTaskResult.FirstErrorMessage;
            }
        }
    }


    private async Task ShowDeleteModal(RepairTaskModel task)
    {
        var message = $"Delete RepairTask?\n\n" +
        $"⏰ Name: {task.Name} ➔ {task.TotalCost.ToString("C", CultureInfo.GetCultureInfo("en-US"))}\n" +
        $"🛠️ Parts: {string.Join(", ", task.Parts.Select(p => $"{p.Name} (${p.Cost:F2})"))}\n";

        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", message);

        if (confirmed)
        {
            await RepairTaskService.DeleteByIdAsync(task.RepairTaskId);
            await LoadRepairTasks();
            ApplyFilters();
        }
    }
}