using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Requests.Customers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MechanicShop.Client.Components.Customers;

public partial class CustomersList
{
    [Inject] private ICustomerService customerService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;



    private readonly Dictionary<string, List<string>> CarMakes = SD.CarMakes;

    private List<CustomerModel> customers = new();
    private List<CustomerModel> filteredCustomers = new();

    private CreateCustomerRequest newCustomerRequest = new();
    private UpdateCustomerRequest updateCustomerRequest = new();

    private bool isEditMode = false;
    private Guid editingCustomerId;
    private string searchTerm = "";
    private string sortBy = "name";
    private bool sortAscending = true;

    private bool _showDialog = false;
    private RenderFragment? _dialogTitle;

    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadCustomers();
        ApplyFilters();
    }

    private async Task LoadCustomers()
    {
        var customerResult = await customerService.GetCustomersAsync();

        if (customerResult.IsSuccess)
            customers = customerResult.Data!;
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
        var filtered = customers.AsEnumerable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            filtered = filtered.Where(c =>
                c.Name!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.Email!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.PhoneNumber!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.Vehicles.Any(v => v.Make.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               v.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               v.LicensePlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        filtered = sortBy switch
        {
            "email" => sortAscending
                ? filtered.OrderBy(c => c.Email)
                : filtered.OrderByDescending(c => c.Email),

            "phone" => sortAscending
                ? filtered.OrderBy(c => c.PhoneNumber)
                : filtered.OrderByDescending(c => c.PhoneNumber),

            "vehicles" => sortAscending
                ? filtered.OrderBy(c => c.Vehicles.Count)
                : filtered.OrderByDescending(c => c.Vehicles.Count),

            _ => sortAscending
                ? filtered.OrderBy(c => c.Name)
                : filtered.OrderByDescending(c => c.Name)
        };

        filteredCustomers = filtered.ToList();
    }

    private void ShowCreateModal()
    {
        isEditMode = false;
        newCustomerRequest = new();
        AddVehicle();
        _dialogTitle = builder => builder.AddContent(0, "Create New Customer");
        _showDialog = true;
    }

    private void ShowEditModal(CustomerModel customer)
    {
        isEditMode = true;
        editingCustomerId = customer.CustomerId;
        updateCustomerRequest = new UpdateCustomerRequest
        {
            Name = customer.Name!,
            Email = customer.Email!,
            PhoneNumber = customer.PhoneNumber!,
            Vehicles = customer.Vehicles.Select(v => new UpdateVehicleRequest
            {
                VehicleId = v.VehicleId,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                LicensePlate = v.LicensePlate
            }).ToList()
        };

        _dialogTitle = builder => builder.AddContent(0, "Edit Customer");
        _showDialog = true;
    }

    private void HideDialog() => _showDialog = false;


    private void AddVehicle()
    {
        if (!isEditMode)
        {
            newCustomerRequest.Vehicles.Add(new CreateVehicleRequest
            {
                Make = "",
                Model = "",
                Year = DateTime.Now.Year,
                LicensePlate = ""
            });
        }
        else
        {
            updateCustomerRequest.Vehicles.Add(new UpdateVehicleRequest
            {
                Make = "",
                Model = "",
                Year = DateTime.Now.Year,
                LicensePlate = ""
            });
        }
    }

    private void RemoveVehicle(int index)
    {
        if (!isEditMode)
        {
            if (index >= 0 && index < newCustomerRequest.Vehicles.Count)
                newCustomerRequest.Vehicles.RemoveAt(index);
        }
        else
        {
            if (index >= 0 && index < updateCustomerRequest.Vehicles.Count)
                updateCustomerRequest.Vehicles.RemoveAt(index);
        }
    }

    private void OnMakeChanged(int index, bool isEdit)
    {
        if (isEdit)
            if (index >= 0 && index < updateCustomerRequest.Vehicles.Count)
                updateCustomerRequest.Vehicles[index].Model = "";
            else
            if (index >= 0 && index < newCustomerRequest.Vehicles.Count)
                newCustomerRequest.Vehicles[index].Model = "";
    }

    private async Task SaveCustomer()
    {
        errorMessage = null;

        if (!isEditMode)
        {
            var result = await customerService.CreateAsync(newCustomerRequest);
            if (result.IsSuccess)
            {
                await LoadCustomers();
                ApplyFilters();
                HideDialog();
            }
            else
            {
                errorMessage = string.Join(", ", result.FirstErrorMessage);
            }
        }
        else
        {
            var result = await customerService.UpdateAsync(editingCustomerId, updateCustomerRequest);
            if (result.IsSuccess)
            {
                await LoadCustomers();
                ApplyFilters();
                HideDialog();
            }
            else
            {
                errorMessage = string.Join(", ", result.FirstErrorMessage);
            }
        }
    }

    private async Task ShowDeleteModal(CustomerModel customer)
    {
        var message = $"Delete customer?\n\n" +
                      $"👤 Name: {customer.Name}\n" +
                      $"📧 Email: {customer.Email}\n" +
                      $"📞 Phone: {customer.PhoneNumber}\n" +
                      $"🚗 Vehicles: {customer.Vehicles.Count}";

        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", message);

        if (confirmed)
        {
            var result = await customerService.DeleteByIdAsync(customer.CustomerId);

            if (result.IsSuccess)
            {
                await LoadCustomers();
                ApplyFilters();
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Failed to delete customer.");
            }
        }
    }


    private async Task DeleteCustomer(Guid customerId)
    {
        var result = await customerService.DeleteByIdAsync(customerId);

        if (result.IsSuccess)
        {
            await LoadCustomers();
            ApplyFilters();
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", "Failed to delete customer.");
        }
    }
}