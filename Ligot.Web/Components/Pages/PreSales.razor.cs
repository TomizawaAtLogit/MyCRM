#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Ligot.Web;
using Ligot.Web.Services;

namespace Ligot.Web.Components.Pages;

public class PreSalesBase : ComponentBase
{
    [Inject] protected PreSalesProposalsApiClient ProposalsApi { get; set; } = null!;
    [Inject] protected PreSalesActivitiesApiClient ActivitiesApi { get; set; } = null!;
    [Inject] protected PreSalesWorkHoursApiClient WorkHoursApi { get; set; } = null!;
    [Inject] protected RequirementDefinitionsApiClient RequirementsApi { get; set; } = null!;
    [Inject] protected CustomerApiClient CustomersApi { get; set; } = null!;
    [Inject] protected OrderApiClient OrdersApi { get; set; } = null!;
    [Inject] protected AdminApiClient AdminApi { get; set; } = null!;
    [Inject] protected AuthorizationService AuthService { get; set; } = null!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] protected LocalizationService Localizer { get; set; } = null!;

    protected PreSalesProposalDto[]? proposals;
    protected PreSalesProposalDto[]? filteredProposals;
    protected CustomerDto[]? customers;
    protected OrderWithCustomerDto[]? allOrders;
    protected OrderWithCustomerDto[]? filteredOrders;
    protected UserDto[]? users;
    protected RequirementDefinitionDto[]? requirements;
    protected PreSalesActivityDto[]? proposalActivities;
    protected bool isReadOnly = false;

    protected bool isLoading = true;
    protected bool showModal = false;
    protected bool showDetailsModal = false;
    protected bool showToast = false;
    protected string toastMessage = "";
    protected string toastType = "success";

    protected PreSalesProposalEditModel editingProposal = new();
    protected PreSalesProposalDto? selectedProposal;

    // Requirement definition create modal state and model
    protected bool showRequirementModal = false;
    protected NewRequirementModel newRequirementModel = new();

    public class NewRequirementModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a customer")]
        public int CustomerId { get; set; }
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
    }

    protected string? filterCustomerId = "";
    protected string? filterStatus = "";
    protected string? filterStage = "";
    protected string? filterAssignedToUserId = "";
    
    // Pagination
    protected int currentPage = 1;
    protected int pageSize = 25;
    protected int totalCount = 0;
    
    protected readonly Dictionary<int, bool> expandedProposals = new();
    protected readonly Dictionary<int, bool> proposalDescriptionExpanded = new();
    protected readonly Dictionary<int, string> proposalActiveTabs = new();
    protected readonly Dictionary<int, bool> loadingProposalActivities = new();
    protected readonly Dictionary<int, PreSalesActivityDto[]> cachedProposalActivities = new();
    protected readonly Dictionary<int, bool> showAddActivityForm = new();
    protected readonly Dictionary<int, NewProposalActivityModel> newActivityModels = new();
    protected readonly Dictionary<int, EditContext> activityCreateContexts = new();
    protected readonly Dictionary<int, bool> isProcessingActivity = new();
    protected readonly Dictionary<int, int?> editingActivityIds = new();

    // Work hours state
    protected readonly Dictionary<int, bool> loadingProposalWorkHours = new();
    protected readonly Dictionary<int, PreSalesWorkHourDto[]> cachedProposalWorkHours = new();
    protected readonly Dictionary<int, bool> showAddWorkHourForm = new();
    protected readonly Dictionary<int, NewProposalWorkHourModel> newWorkHourModels = new();
    protected readonly Dictionary<int, EditContext> workHourCreateContexts = new();
    protected readonly Dictionary<int, bool> isProcessingWorkHour = new();
    protected readonly Dictionary<int, int?> editingWorkHourIds = new();

    protected override async Task OnInitializedAsync()
    {
        isReadOnly = await AuthService.IsPageReadOnlyAsync("PreSales");
        await LoadData();
    }

    protected async Task LoadData()
    {
        isLoading = true;

        proposals = await ProposalsApi.GetProposalsAsync();
        customers = await CustomersApi.GetCustomersAsync();
        users = await AdminApi.GetUsersAsync();
        requirements = await RequirementsApi.GetRequirementDefinitionsAsync();
        allOrders = await OrdersApi.GetOrdersAsync();

        ApplyFilters();
        isLoading = false;
    }

    protected void ApplyFilters()
    {
        filteredProposals = proposals;

        if (!string.IsNullOrEmpty(filterCustomerId) && int.TryParse(filterCustomerId, out var custId))
        {
            filteredProposals = filteredProposals?.Where(p => p.CustomerId == custId).ToArray();
        }

        if (!string.IsNullOrEmpty(filterStatus) && Enum.TryParse<PreSalesStatus>(filterStatus, out var status))
        {
            filteredProposals = filteredProposals?.Where(p => p.Status == status).ToArray();
        }

        if (!string.IsNullOrEmpty(filterStage) && Enum.TryParse<PreSalesStage>(filterStage, out var stage))
        {
            filteredProposals = filteredProposals?.Where(p => p.Stage == stage).ToArray();
        }

        if (!string.IsNullOrEmpty(filterAssignedToUserId) && int.TryParse(filterAssignedToUserId, out var userId))
        {
            filteredProposals = filteredProposals?.Where(p => p.AssignedToUserId == userId).ToArray();
        }
        
        // Update total count and reset to first page
        totalCount = filteredProposals?.Length ?? 0;
        currentPage = 1;
    }

    protected void OnCustomerFilterChanged(ChangeEventArgs e)
    {
        filterCustomerId = e.Value?.ToString();
        ApplyFilters();
    }

    protected void OnStatusFilterChanged(ChangeEventArgs e)
    {
        filterStatus = e.Value?.ToString();
        ApplyFilters();
    }

    protected void OnStageFilterChanged(ChangeEventArgs e)
    {
        filterStage = e.Value?.ToString();
        ApplyFilters();
    }

    protected void OnAssignedToFilterChanged(ChangeEventArgs e)
    {
        filterAssignedToUserId = e.Value?.ToString();
        ApplyFilters();
    }

    protected void ShowCreateModal()
    {
        editingProposal = new PreSalesProposalEditModel();
        showModal = true;
    }

    protected void ShowEditModal(PreSalesProposalDto proposal)
    {
        editingProposal = new PreSalesProposalEditModel
        {
            Id = proposal.Id,
            Title = proposal.Title,
            Description = proposal.Description,
            CustomerId = proposal.CustomerId,
            RequirementDefinitionId = proposal.RequirementDefinitionId,
            CustomerOrderId = proposal.CustomerOrderId,
            Status = proposal.Status,
            Stage = proposal.Stage,
            AssignedToUserId = proposal.AssignedToUserId,
            EstimatedValue = proposal.EstimatedValue,
            ProbabilityPercentage = proposal.ProbabilityPercentage,
            ExpectedCloseDate = proposal.ExpectedCloseDate,
            Notes = proposal.Notes
        };
        FilterOrdersByCustomer();
        showModal = true;
    }

    protected async Task ShowDetailsModal(PreSalesProposalDto proposal)
    {
        selectedProposal = proposal;
        proposalActivities = await LoadActivitiesIfNeededAsync(proposal.Id, forceRefresh: true);
        showDetailsModal = true;
    }

    protected async Task ToggleProposalExpansionAsync(int proposalId)
    {
        var isExpanded = expandedProposals.TryGetValue(proposalId, out var expanded) && expanded;
        expandedProposals[proposalId] = !isExpanded;

        if (!proposalActiveTabs.ContainsKey(proposalId))
        {
            proposalActiveTabs[proposalId] = "activities";
        }

        if (expandedProposals[proposalId])
        {
            var activeTab = GetProposalTab(proposalId);
            if (activeTab == "workhours")
            {
                await LoadWorkHoursIfNeededAsync(proposalId);
            }
            else
            {
                await LoadActivitiesIfNeededAsync(proposalId);
            }
        }
    }

    protected async Task StartEditActivity(int proposalId, PreSalesActivityDto activity)
    {
        EnsureActivityModel(proposalId);
        newActivityModels[proposalId] = new NewProposalActivityModel
        {
            ActivityDate = activity.ActivityDate,
            Summary = activity.Summary,
            Description = activity.Description,
            NextAction = activity.NextAction,
            ActivityType = activity.ActivityType,
            PerformedBy = activity.PerformedBy
        };
        activityCreateContexts[proposalId] = new EditContext(newActivityModels[proposalId]);
        editingActivityIds[proposalId] = activity.Id;
        showAddActivityForm[proposalId] = true;
        await InvokeAsync(StateHasChanged);
    }

    protected void CancelActivityEdit(int proposalId)
    {
        editingActivityIds[proposalId] = null;
        showAddActivityForm[proposalId] = false;
        ResetActivityModel(proposalId);
    }

    protected async Task DeleteActivityAsync(int proposalId, int activityId)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", Localizer.GetString("Confirm Delete Activity")))
        {
            return;
        }

        var success = await ActivitiesApi.DeleteActivityAsync(activityId);
        if (success)
        {
            ShowToast(Localizer.GetString("Activity deleted successfully"), "success");
            if (editingActivityIds.TryGetValue(proposalId, out var editingId) && editingId == activityId)
            {
                editingActivityIds[proposalId] = null;
                showAddActivityForm[proposalId] = false;
                ResetActivityModel(proposalId);
            }
            var refreshed = await LoadActivitiesIfNeededAsync(proposalId, forceRefresh: true);
            if (selectedProposal?.Id == proposalId)
            {
                proposalActivities = refreshed;
            }
        }
        else
        {
            ShowToast(Localizer.GetString("Error deleting activity"), "danger");
        }
    }


    protected async Task ShowAddActivityFormAsync(int proposalId)
    {
        EnsureActivityModel(proposalId);
        editingActivityIds[proposalId] = null;
        showAddActivityForm[proposalId] = true;
        await InvokeAsync(StateHasChanged);
    }

    protected void HideAddActivityForm(int proposalId)
    {
        showAddActivityForm[proposalId] = false;
        editingActivityIds[proposalId] = null;
    }

    protected void EnsureActivityModel(int proposalId)
    {
        if (!newActivityModels.ContainsKey(proposalId))
        {
            newActivityModels[proposalId] = new NewProposalActivityModel();
            activityCreateContexts[proposalId] = new EditContext(newActivityModels[proposalId]);
        }
        else if (!activityCreateContexts.ContainsKey(proposalId))
        {
            activityCreateContexts[proposalId] = new EditContext(newActivityModels[proposalId]);
        }
    }

    protected void ResetActivityModel(int proposalId)
    {
        newActivityModels[proposalId] = new NewProposalActivityModel();
        activityCreateContexts[proposalId] = new EditContext(newActivityModels[proposalId]);
    }

    protected async Task SubmitActivityFormAsync(int proposalId)
    {
        if (!activityCreateContexts.TryGetValue(proposalId, out var context) || !context.Validate())
        {
            ShowToast(Localizer.GetString("Please correct the validation errors"), "danger");
            return;
        }

        if (!newActivityModels.TryGetValue(proposalId, out var model))
        {
            ShowToast(Localizer.GetString("Error creating activity"), "danger");
            return;
        }

        var editingId = editingActivityIds.TryGetValue(proposalId, out var id) ? id : null;
        isProcessingActivity[proposalId] = true;
        try
        {
            bool succeeded;
            if (editingId.HasValue)
            {
                var dto = new UpdatePreSalesActivityDto(
                    editingId.Value,
                    proposalId,
                    model.Summary,
                    string.IsNullOrWhiteSpace(model.Description) ? null : model.Description,
                    string.IsNullOrWhiteSpace(model.NextAction) ? null : model.NextAction,
                    string.IsNullOrWhiteSpace(model.ActivityType) ? null : model.ActivityType,
                    string.IsNullOrWhiteSpace(model.PerformedBy) ? null : model.PerformedBy,
                    model.ActivityDate);

                succeeded = await ActivitiesApi.UpdateActivityAsync(dto);
                ShowToast(Localizer.GetString(succeeded ? "Activity updated successfully" : "Failed to update activity"), succeeded ? "success" : "danger");
            }
            else
            {
                var dto = new CreatePreSalesActivityDto(
                    proposalId,
                    model.Summary,
                    string.IsNullOrWhiteSpace(model.Description) ? null : model.Description,
                    string.IsNullOrWhiteSpace(model.NextAction) ? null : model.NextAction,
                    string.IsNullOrWhiteSpace(model.ActivityType) ? null : model.ActivityType,
                    string.IsNullOrWhiteSpace(model.PerformedBy) ? null : model.PerformedBy,
                    model.ActivityDate);

                var created = await ActivitiesApi.CreateActivityAsync(dto);
                succeeded = created != null;
                ShowToast(Localizer.GetString(succeeded ? "Activity created successfully" : "Failed to create activity"), succeeded ? "success" : "danger");
            }

            if (!succeeded)
            {
                return;
            }

            var refreshed = await LoadActivitiesIfNeededAsync(proposalId, forceRefresh: true);
            if (selectedProposal?.Id == proposalId)
            {
                proposalActivities = refreshed;
            }

            showAddActivityForm[proposalId] = false;
            editingActivityIds[proposalId] = null;
            ResetActivityModel(proposalId);
        }
        catch (Exception ex)
        {
            ShowToast(Localizer.GetString(editingId.HasValue ? "Error updating activity" : "Error creating activity") + $": {ex.Message}", "danger");
        }
        finally
        {
            isProcessingActivity[proposalId] = false;
        }
    }

    protected async Task<PreSalesActivityDto[]> LoadActivitiesIfNeededAsync(int proposalId, bool forceRefresh = false)
    {
        if (!forceRefresh && cachedProposalActivities.TryGetValue(proposalId, out var cached))
        {
            return cached;
        }

        loadingProposalActivities[proposalId] = true;
        try
        {
            var activities = await ActivitiesApi.GetActivitiesAsync(proposalId) ?? Array.Empty<PreSalesActivityDto>();
            cachedProposalActivities[proposalId] = activities;
            return activities;
        }
        catch (Exception ex)
        {
            cachedProposalActivities[proposalId] = Array.Empty<PreSalesActivityDto>();
            ShowToast(Localizer.GetString("Error loading activities") + $": {ex.Message}", "danger");
            return Array.Empty<PreSalesActivityDto>();
        }
        finally
        {
            loadingProposalActivities[proposalId] = false;
        }
    }

    protected bool IsProposalLoadingActivities(int proposalId)
    {
        return loadingProposalActivities.TryGetValue(proposalId, out var loading) && loading;
    }

    protected void HideModal()
    {
        showModal = false;
        editingProposal = new();
        filteredOrders = null;
    }

    protected void ShowCreateRequirementModal()
    {
        if (editingProposal.CustomerId == 0)
        {
            ShowToast(Localizer.GetString("Please select a customer before adding requirement definition"), "danger");
            return;
        }
        newRequirementModel = new NewRequirementModel { CustomerId = editingProposal.CustomerId };
        showRequirementModal = true;
    }

    protected void HideRequirementModal()
    {
        showRequirementModal = false;
        newRequirementModel = new NewRequirementModel();
    }

    protected async Task SaveRequirementDefinition()
    {
        try
        {
            var created = await RequirementsApi.CreateRequirementDefinitionAsync(
                new CreateRequirementDefinitionDto(
                    newRequirementModel.Title,
                    string.IsNullOrWhiteSpace(newRequirementModel.Description) ? null : newRequirementModel.Description,
                    newRequirementModel.CustomerId,
                    newRequirementModel.Category,
                    newRequirementModel.Priority,
                    newRequirementModel.Status));

            if (created != null)
            {
                ShowToast(Localizer.GetString("Requirement definition created"), "success");
                // reload available requirement definitions and assign the new one to the proposal
                requirements = await RequirementsApi.GetRequirementDefinitionsAsync();
                editingProposal.RequirementDefinitionId = created.Id;
                HideRequirementModal();
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                ShowToast(Localizer.GetString("Failed to create requirement definition"), "danger");
            }
        }
        catch (Exception ex)
        {
            ShowToast(Localizer.GetString("Error creating requirement definition") + $": {ex.Message}", "danger");
        }
    }

    protected void OnProposalCustomerChanged()
    {
        // Clear order when customer changes (lifecycle hook)
        editingProposal.CustomerOrderId = null;
        FilterOrdersByCustomer();
    }

    protected void FilterOrdersByCustomer()
    {
        filteredOrders = FilterOrdersByCustomerId(editingProposal.CustomerId);
    }

    protected OrderWithCustomerDto[]? FilterOrdersByCustomerId(int customerId)
    {
        if (customerId > 0 && allOrders != null)
        {
            return allOrders.Where(o => o.CustomerId == customerId).ToArray();
        }
        return null;
    }

    protected void HideDetailsModal()
    {
        showDetailsModal = false;
        selectedProposal = null;
        proposalActivities = null;
    }

    protected void OnAssignedToChanged()
    {
        // Lifecycle hook to ensure proper binding of AssignedToUserId
        // No additional logic needed - just ensures binding completes
    }

    protected async Task SaveProposal()
    {
        try
        {
            if (editingProposal.Id == 0)
            {
                var dto = new CreatePreSalesProposalDto(
                    editingProposal.Title,
                    editingProposal.Description,
                    editingProposal.CustomerId,
                    editingProposal.RequirementDefinitionId,
                    editingProposal.CustomerOrderId,
                    editingProposal.Status,
                    editingProposal.Stage,
                    editingProposal.AssignedToUserId,
                    editingProposal.EstimatedValue,
                    editingProposal.ProbabilityPercentage,
                    editingProposal.ExpectedCloseDate,
                    editingProposal.Notes
                );
                var created = await ProposalsApi.CreateProposalAsync(dto);
                if (created != null)
                {
                    ShowToast(Localizer.GetString("Proposal created successfully!"), "success");
                    await LoadData();
                    HideModal();
                }
            }
            else
            {
                var dto = new UpdatePreSalesProposalDto(
                    editingProposal.Id,
                    editingProposal.Title,
                    editingProposal.Description,
                    editingProposal.CustomerId,
                    editingProposal.RequirementDefinitionId,
                    editingProposal.CustomerOrderId,
                    editingProposal.Status,
                    editingProposal.Stage,
                    editingProposal.AssignedToUserId,
                    editingProposal.EstimatedValue,
                    editingProposal.ProbabilityPercentage,
                    editingProposal.ExpectedCloseDate,
                    editingProposal.Notes
                );
                var success = await ProposalsApi.UpdateProposalAsync(dto);
                if (success)
                {
                    ShowToast(Localizer.GetString("Proposal updated successfully!"), "success");
                    await LoadData();
                    HideModal();
                }
            }
        }
        catch (Exception ex)
        {
            ShowToast(Localizer.GetString("Error saving proposal") + $": {ex.Message}", "danger");
        }
    }

    protected async Task DeleteProposal(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", Localizer.GetString("Confirm Delete Proposal")))
        {
            var success = await ProposalsApi.DeleteProposalAsync(id);
            if (success)
            {
                ShowToast(Localizer.GetString("Proposal deleted successfully!"), "success");
                await LoadData();
            }
            else
            {
                ShowToast(Localizer.GetString("Error deleting proposal"), "danger");
            }
        }
    }

    protected void ShowToast(string message, string type)
    {
        toastMessage = message;
        toastType = type;
        showToast = true;
    }

    protected string GetStatusBadgeColor(PreSalesStatus status)
    {
        return status switch
        {
            PreSalesStatus.Draft => "secondary",
            PreSalesStatus.InReview => "info",
            PreSalesStatus.Pending => "warning",
            PreSalesStatus.Approved => "success",
            PreSalesStatus.Rejected => "danger",
            PreSalesStatus.Closed => "dark",
            _ => "secondary"
        };
    }

    protected string GetStatusLabel(PreSalesStatus status) =>
        Localizer.GetString($"PreSalesStatus.{status}");

    protected string GetStageLabel(PreSalesStage stage) =>
        Localizer.GetString($"PreSalesStage.{stage}");

    protected string GetProposalTab(int proposalId)
    {
        return proposalActiveTabs.TryGetValue(proposalId, out var tab) ? tab : "activities";
    }

    protected async Task SetProposalTabAsync(int proposalId, string tab)
    {
        proposalActiveTabs[proposalId] = tab;
        if (tab == "workhours")
        {
            await LoadWorkHoursIfNeededAsync(proposalId);
        }
    }

    protected void SetProposalTab(int proposalId, string tab)
    {
        proposalActiveTabs[proposalId] = tab;
    }

    protected void ToggleProposalDescription(int proposalId)
    {
        if (!proposalDescriptionExpanded.ContainsKey(proposalId))
        {
            proposalDescriptionExpanded[proposalId] = false;
        }

        proposalDescriptionExpanded[proposalId] = !proposalDescriptionExpanded[proposalId];
    }

    protected string Truncate(string? s, int length)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return s.Length <= length ? s : s.Substring(0, length) + "...";
    }

    protected class NewProposalActivityModel
    {
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(500)]
        public string Summary { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? NextAction { get; set; }

        [MaxLength(200)]
        public string? ActivityType { get; set; }

        [MaxLength(200)]
        public string? PerformedBy { get; set; }
    }

    public class PreSalesProposalEditModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a customer")]
        public int CustomerId { get; set; }

        public int? RequirementDefinitionId { get; set; }

        public int? CustomerOrderId { get; set; }

        public PreSalesStatus Status { get; set; } = PreSalesStatus.Draft;

        public PreSalesStage Stage { get; set; } = PreSalesStage.InitialContact;

        public int? AssignedToUserId { get; set; }

        public decimal? EstimatedValue { get; set; }

        [Range(0, 100)]
        public int? ProbabilityPercentage { get; set; }

        public DateTime? ExpectedCloseDate { get; set; }

        public string? Notes { get; set; }
    }

    protected PreSalesProposalDto[]? GetPaginatedProposals()
    {
        if (filteredProposals == null || filteredProposals.Length == 0)
            return filteredProposals;

        var startIndex = (currentPage - 1) * pageSize;
        return filteredProposals.Skip(startIndex).Take(pageSize).ToArray();
    }

    protected int GetTotalPages()
    {
        if (totalCount == 0) return 1;
        return (int)Math.Ceiling((double)totalCount / pageSize);
    }

    protected void GoToPage(int page)
    {
        var totalPages = GetTotalPages();
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
        }
    }

    protected void NextPage()
    {
        if (currentPage < GetTotalPages())
            currentPage++;
    }

    protected void PreviousPage()
    {
        if (currentPage > 1)
            currentPage--;
    }

    protected void OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var newSize) && newSize > 0)
        {
            pageSize = newSize;
            currentPage = 1; // Reset to first page when changing page size
        }
    }

    protected string GetPaginationInfo()
    {
        if (totalCount == 0)
            return "0 items";

        var startItem = (currentPage - 1) * pageSize + 1;
        var endItem = Math.Min(currentPage * pageSize, totalCount);
        return $"{startItem}-{endItem} of {totalCount} items";
    }

    // Work hours methods
    protected async Task<PreSalesWorkHourDto[]> LoadWorkHoursIfNeededAsync(int proposalId, bool forceRefresh = false)
    {
        if (!forceRefresh && cachedProposalWorkHours.TryGetValue(proposalId, out var cached))
        {
            return cached;
        }

        loadingProposalWorkHours[proposalId] = true;
        try
        {
            var workHours = await WorkHoursApi.GetWorkHoursAsync(proposalId) ?? Array.Empty<PreSalesWorkHourDto>();
            cachedProposalWorkHours[proposalId] = workHours;
            return workHours;
        }
        catch (Exception ex)
        {
            cachedProposalWorkHours[proposalId] = Array.Empty<PreSalesWorkHourDto>();
            ShowToast(Localizer.GetString("Error loading work hours") + $": {ex.Message}", "danger");
            return Array.Empty<PreSalesWorkHourDto>();
        }
        finally
        {
            loadingProposalWorkHours[proposalId] = false;
        }
    }

    protected bool IsProposalLoadingWorkHours(int proposalId)
    {
        return loadingProposalWorkHours.TryGetValue(proposalId, out var loading) && loading;
    }

    protected async Task ShowAddWorkHourFormAsync(int proposalId)
    {
        EnsureWorkHourModel(proposalId);
        editingWorkHourIds[proposalId] = null;
        showAddWorkHourForm[proposalId] = true;
        await InvokeAsync(StateHasChanged);
    }

    protected void HideAddWorkHourForm(int proposalId)
    {
        showAddWorkHourForm[proposalId] = false;
        editingWorkHourIds[proposalId] = null;
    }

    protected void EnsureWorkHourModel(int proposalId)
    {
        if (!newWorkHourModels.ContainsKey(proposalId))
        {
            newWorkHourModels[proposalId] = new NewProposalWorkHourModel();
            workHourCreateContexts[proposalId] = new EditContext(newWorkHourModels[proposalId]);
        }
        else if (!workHourCreateContexts.ContainsKey(proposalId))
        {
            workHourCreateContexts[proposalId] = new EditContext(newWorkHourModels[proposalId]);
        }
    }

    protected void ResetWorkHourModel(int proposalId)
    {
        newWorkHourModels[proposalId] = new NewProposalWorkHourModel();
        workHourCreateContexts[proposalId] = new EditContext(newWorkHourModels[proposalId]);
    }

    protected async Task StartEditWorkHour(int proposalId, PreSalesWorkHourDto workHour)
    {
        EnsureWorkHourModel(proposalId);
        newWorkHourModels[proposalId] = new NewProposalWorkHourModel
        {
            Title = workHour.Title,
            Description = workHour.Description,
            NumberOfPeople = workHour.NumberOfPeople,
            WorkingHours = workHour.WorkingHours,
            HourlyWage = workHour.HourlyWage
        };
        workHourCreateContexts[proposalId] = new EditContext(newWorkHourModels[proposalId]);
        editingWorkHourIds[proposalId] = workHour.Id;
        showAddWorkHourForm[proposalId] = true;
        await InvokeAsync(StateHasChanged);
    }

    protected void CancelWorkHourEdit(int proposalId)
    {
        editingWorkHourIds[proposalId] = null;
        showAddWorkHourForm[proposalId] = false;
        ResetWorkHourModel(proposalId);
    }

    protected async Task SubmitWorkHourFormAsync(int proposalId)
    {
        if (!workHourCreateContexts.TryGetValue(proposalId, out var context) || !context.Validate())
        {
            ShowToast(Localizer.GetString("Please correct the validation errors"), "danger");
            return;
        }

        if (!newWorkHourModels.TryGetValue(proposalId, out var model))
        {
            ShowToast(Localizer.GetString("Error creating work hour"), "danger");
            return;
        }

        var editingId = editingWorkHourIds.TryGetValue(proposalId, out var id) ? id : null;
        isProcessingWorkHour[proposalId] = true;
        try
        {
            bool succeeded;
            if (editingId.HasValue)
            {
                var dto = new UpdatePreSalesWorkHourDto(
                    editingId.Value,
                    proposalId,
                    model.Title,
                    string.IsNullOrWhiteSpace(model.Description) ? null : model.Description,
                    model.NumberOfPeople,
                    model.WorkingHours,
                    model.HourlyWage);

                succeeded = await WorkHoursApi.UpdateWorkHourAsync(dto);
                ShowToast(Localizer.GetString(succeeded ? "Work hour updated successfully" : "Failed to update work hour"), succeeded ? "success" : "danger");
            }
            else
            {
                var dto = new CreatePreSalesWorkHourDto(
                    proposalId,
                    model.Title,
                    string.IsNullOrWhiteSpace(model.Description) ? null : model.Description,
                    model.NumberOfPeople,
                    model.WorkingHours,
                    model.HourlyWage);

                var created = await WorkHoursApi.CreateWorkHourAsync(dto);
                succeeded = created != null;
                ShowToast(Localizer.GetString(succeeded ? "Work hour created successfully" : "Failed to create work hour"), succeeded ? "success" : "danger");
            }

            if (!succeeded)
            {
                return;
            }

            await LoadWorkHoursIfNeededAsync(proposalId, forceRefresh: true);
            showAddWorkHourForm[proposalId] = false;
            editingWorkHourIds[proposalId] = null;
            ResetWorkHourModel(proposalId);
        }
        catch (Exception ex)
        {
            ShowToast(Localizer.GetString(editingId.HasValue ? "Error updating work hour" : "Error creating work hour") + $": {ex.Message}", "danger");
        }
        finally
        {
            isProcessingWorkHour[proposalId] = false;
        }
    }

    protected async Task DeleteWorkHourAsync(int proposalId, int workHourId)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", Localizer.GetString("Confirm Delete Work Hour")))
        {
            return;
        }

        var success = await WorkHoursApi.DeleteWorkHourAsync(workHourId);
        if (success)
        {
            ShowToast(Localizer.GetString("Work hour deleted successfully"), "success");
            if (editingWorkHourIds.TryGetValue(proposalId, out var editingId) && editingId == workHourId)
            {
                editingWorkHourIds[proposalId] = null;
                showAddWorkHourForm[proposalId] = false;
                ResetWorkHourModel(proposalId);
            }
            await LoadWorkHoursIfNeededAsync(proposalId, forceRefresh: true);
        }
        else
        {
            ShowToast(Localizer.GetString("Error deleting work hour"), "danger");
        }
    }

    protected decimal CalculateTotalEstimatedCost(int proposalId)
    {
        if (cachedProposalWorkHours.TryGetValue(proposalId, out var workHours))
        {
            return workHours.Sum(w => w.TotalCost);
        }
        return 0;
    }

    protected string FormatCurrency(decimal amount)
    {
        return $"¥{amount:N0}";
    }

    public class NewProposalWorkHourModel
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of people must be at least 1")]
        public int NumberOfPeople { get; set; } = 1;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Working hours must be at least 1")]
        public int WorkingHours { get; set; } = 1;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Hourly wage must be at least 1")]
        public int HourlyWage { get; set; } = 0;
    }
}

