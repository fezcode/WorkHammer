using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using WorkHammer.Models;
using WorkHammer.Services;

namespace WorkHammer.ViewModels;

public enum ConfirmationResult { Cancel, No, Yes }
public enum JobSortOption { Name, Status, DateApplied, LastUpdate }

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly JobDataService _jobService;
    private bool _isRestoringSelection; // Flag to prevent recursion when cancelling nav
    private List<JobApplication> _allJobs = new();

    [ObservableProperty]
    private string? _currentDataPath;

    [ObservableProperty]
    private ObservableCollection<JobApplication> _jobs = new();

    [ObservableProperty]
    private JobApplication? _selectedJob;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private JobStatus? _statusFilter;

    [ObservableProperty]
    private JobSortOption _currentSort = JobSortOption.LastUpdate;

    [ObservableProperty]
    private bool _isAscending = false;

    [ObservableProperty]
    private string _statsText = "No applications loaded";

    public bool IsNameSortActive => CurrentSort == JobSortOption.Name;
    public bool IsStatusSortActive => CurrentSort == JobSortOption.Status;
    public bool IsDateSortActive => CurrentSort == JobSortOption.DateApplied;
    public bool IsUpdateSortActive => CurrentSort == JobSortOption.LastUpdate;

    public string SortDirectionIcon => IsAscending ? "M7 14l5-5 5 5H7z" : "M7 10l5 5 5-5H7z"; // Up/Down arrows

    public Symbol NameSortSymbol => IsNameSortActive ? (IsAscending ? Symbol.ChevronUp : Symbol.ChevronDown) : Symbol.Forward;
    public Symbol StatusSortSymbol => IsStatusSortActive ? (IsAscending ? Symbol.ChevronUp : Symbol.ChevronDown) : Symbol.Forward;
    public Symbol DateSortSymbol => IsDateSortActive ? (IsAscending ? Symbol.ChevronUp : Symbol.ChevronDown) : Symbol.Forward;
    public Symbol UpdateSortSymbol => IsUpdateSortActive ? (IsAscending ? Symbol.ChevronUp : Symbol.ChevronDown) : Symbol.Forward;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty] private string _newTechTag = string.Empty;

            [ObservableProperty] private int _totalCount;
            [ObservableProperty] private int _appliedCount;
            [ObservableProperty] private int _interviewingCount;
            [ObservableProperty] private int _offersCount;
            [ObservableProperty] private int _rejectedCount;
            [ObservableProperty] private int _ghostedCount;
        public JobStatus[] JobStatuses => Enum.GetValues<JobStatus>();

            // Actions to request confirmation from the View

            public Func<string, Task<ConfirmationResult>>? ConfirmSaveAction { get; set; }

            public Func<string, Task<bool>>? ConfirmDeleteAction { get; set; }

            public Action? ShowAboutAction { get; set; }

    

            public MainWindowViewModel()

            {

                _jobService = new JobDataService();

                var defaultPath = Path.GetFullPath("Data");

                if (Directory.Exists(defaultPath))

                {

                    CurrentDataPath = defaultPath;

                    _ = LoadJobsAsync();

                }

            }

    

            [RelayCommand]

            private void ShowAbout()

            {

                ShowAboutAction?.Invoke();

            }

    

                    partial void OnSearchTextChanged(string value) => ApplyFilter();

    

                    partial void OnStatusFilterChanged(JobStatus? value) => ApplyFilter();

    

                    partial void OnCurrentSortChanged(JobSortOption value) => ApplyFilter();

    

                    partial void OnIsAscendingChanged(bool value) => ApplyFilter();

    

                

    

                    [RelayCommand]

    

                

    

            

    

                

    

            

    

                                private void SetSort(JobSortOption option)

    

            

    

                

    

            

    

                                {

    

            

    

                

    

            

    

                                    if (CurrentSort == option)

    

            

    

                

    

            

    

                                    {

    

            

    

                

    

            

    

                                        IsAscending = !IsAscending;

    

            

    

                

    

            

    

                                    }

    

            

    

                

    

            

    

                                    else 

    

            

    

                

    

            

    

                                    {

    

            

    

                

    

            

    

                                        CurrentSort = option;

    

            

    

                

    

            

    

                                        // Defaults: Names asc, Dates desc

    

            

    

                

    

            

    

                                        IsAscending = (option == JobSortOption.Name);

    

            

    

                

    

            

    

                                    }

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(IsNameSortActive));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(IsStatusSortActive));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(IsDateSortActive));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(IsUpdateSortActive));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(NameSortSymbol));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(StatusSortSymbol));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(DateSortSymbol));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(UpdateSortSymbol));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                            OnPropertyChanged(nameof(SortDirectionIcon));

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                    

    

            

    

                

    

            

    

                                }

    

            

    

                

    

            

    

                            

    

            

    

                

    

            

    

                                [RelayCommand]

    

            

    

                

    

            

    

                                private void AddTechTag()

    

            

    

                

    

            

    

                            

    

            

    

                    {

    

            

    

                        if (!string.IsNullOrWhiteSpace(NewTechTag) && SelectedJob != null)

    

            

    

                        {

    

            

    

                            var tag = NewTechTag.Trim();

    

            

    

                            if (!SelectedJob.TechStack.Contains(tag))

    

            

    

                            {

    

            

    

                                SelectedJob.TechStack.Add(tag);

    

            

    

                            }

    

            

    

                            NewTechTag = string.Empty;

    

            

    

                        }

    

            

    

                    }

    

            

    

                

    

            

    

                    [RelayCommand]

    

            

    

                    private void RemoveTechTag(string tag)

    

            

    

                    {

    

            

    

                        SelectedJob?.TechStack.Remove(tag);

    

            

    

                    }

    

            

    

                

    

            

    

                    [RelayCommand]

    

            

    

                    private void ClearFilter()

    

            

    

                    {

    

            

    

                        StatusFilter = null;

    

            

    

                    }

    

            

    

                

    

            

    

                    [RelayCommand]

    

            

    

                    private void OpenExplorer()

    

            

    

                    {

    

            

    

                        if (!string.IsNullOrEmpty(CurrentDataPath) && Directory.Exists(CurrentDataPath))

    

            

    

                        {

    

            

    

                            System.Diagnostics.Process.Start("explorer", CurrentDataPath);

    

            

    

                        }

    

            

    

                    }

    

            

    

                

    

            

    

                    private void ApplyFilter()

    

            

    

                    {

    

            

    

                        var filtered = _allJobs.AsEnumerable();

    

            

    

                

    

            

    

                        if (!string.IsNullOrWhiteSpace(SearchText))

    

            

    

                        {

    

            

    

                            filtered = filtered.Where(j => 

    

            

    

                                j.Company.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 

    

            

    

                                j.Role.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

    

            

    

                        }

    

            

    

                

    

            

    

                        if (StatusFilter.HasValue)

    

            

    

                        {

    

            

    

                            filtered = filtered.Where(j => j.Status == StatusFilter.Value);

    

            

    

                        }

    

            

    

                

    

            

    

                        var results = filtered.ToList();

    

            

    

                

    

            

    

                        // Sorting

    

            

    

                        IEnumerable<JobApplication> sorted = CurrentSort switch

    

            

    

                        {

    

            

    

                            JobSortOption.Name => IsAscending ? results.OrderBy(j => j.Company) : results.OrderByDescending(j => j.Company),

    

            

    

                            JobSortOption.Status => IsAscending ? results.OrderBy(j => j.Status) : results.OrderByDescending(j => j.Status),

    

            

    

                            JobSortOption.DateApplied => IsAscending ? results.OrderBy(j => j.AppliedDate) : results.OrderByDescending(j => j.AppliedDate),

    

            

    

                            JobSortOption.LastUpdate => IsAscending ? results.OrderBy(j => j.UpdatedDate) : results.OrderByDescending(j => j.UpdatedDate),

    

            

    

                            _ => results

    

            

    

                        };

    

            

    

                

    

            

    

                        Jobs = new ObservableCollection<JobApplication>(sorted);

    

            

    

                        UpdateStats();

    

            

    

                    }

            private void UpdateStats()
            {
                TotalCount = _allJobs.Count;
                AppliedCount = _allJobs.Count(j => j.Status == JobStatus.Applied);
                InterviewingCount = _allJobs.Count(j => j.Status == JobStatus.Interviewing);
                OffersCount = _allJobs.Count(j => j.Status == JobStatus.Offer);
                RejectedCount = _allJobs.Count(j => j.Status == JobStatus.Rejected);
                GhostedCount = _allJobs.Count(j => j.Status == JobStatus.Ghosted);
    
                if (!_allJobs.Any())
                {
                    StatsText = "No applications";
                    return;
                }
    
                StatsText = $"Total: {TotalCount} | Applied: {AppliedCount} | Interviewing: {InterviewingCount} | Offers: {OffersCount} | Rejected: {RejectedCount}";
            }
        [RelayCommand]
    private void EditCurrent()
    {
        if (SelectedJob != null) IsEditing = true;
    }

    [RelayCommand]
    private async Task CancelEdit()
    {
        if (SelectedJob != null && SelectedJob.IsDirty && !string.IsNullOrEmpty(CurrentDataPath))
        {
            var originalJob = await _jobService.LoadJobAsync(CurrentDataPath, SelectedJob.Id);
            if (originalJob != null)
            {
                originalJob.IsDirty = false;
                int index = _allJobs.FindIndex(j => j.Id == SelectedJob.Id);
                if (index != -1)
                {
                    _allJobs[index] = originalJob;
                    ApplyFilter();
                    SelectedJob = originalJob;
                }
            }
            else
            {
                _allJobs.Remove(SelectedJob);
                ApplyFilter();
                SelectedJob = Jobs.FirstOrDefault();
            }
        }
        IsEditing = false;
    }

    // Called when SelectedJob changes
    async partial void OnSelectedJobChanged(JobApplication? oldValue, JobApplication? newValue)
    {
        if (_isRestoringSelection) return;

        IsEditing = false; // Always start in View mode

        // Check if we need to save the OLD value
        if (oldValue != null && oldValue.IsDirty)
        {
            if (ConfirmSaveAction != null)
            {
                var result = await ConfirmSaveAction.Invoke(oldValue.Company);
                
                if (result == ConfirmationResult.Cancel)
                {
                    _isRestoringSelection = true;
                    SelectedJob = oldValue; 
                    _isRestoringSelection = false;
                    return; 
                }
                else if (result == ConfirmationResult.Yes)
                {
                    if (!string.IsNullOrEmpty(CurrentDataPath))
                    {
                        await _jobService.SaveJobAsync(CurrentDataPath, oldValue);
                        oldValue.IsDirty = false;
                    }
                }
                else if (result == ConfirmationResult.No)
                {
                    if (!string.IsNullOrEmpty(CurrentDataPath))
                    {
                        var originalJob = await _jobService.LoadJobAsync(CurrentDataPath, oldValue.Id);
                        if (originalJob != null)
                        {
                            originalJob.IsDirty = false;
                            int index = _allJobs.FindIndex(j => j.Id == oldValue.Id);
                            if (index != -1)
                            {
                                _allJobs[index] = originalJob;
                                ApplyFilter();
                            }
                        }
                        else 
                        {
                            _allJobs.Remove(oldValue);
                            ApplyFilter();
                        }
                    }
                }
            }
        }
    }

    [RelayCommand]
    public async Task LoadJobsAsync()
    {
        if (string.IsNullOrEmpty(CurrentDataPath)) return;

        _allJobs = await _jobService.LoadJobsAsync(CurrentDataPath);
        foreach(var job in _allJobs) job.IsDirty = false;

        ApplyFilter();
        SelectedJob = Jobs.FirstOrDefault();
    }

    [RelayCommand]
    private void AddJob()
    {
        if (string.IsNullOrEmpty(CurrentDataPath)) return;

        var newJob = new JobApplication
        {
            Company = "New Company",
            Role = "New Role",
            AppliedDate = DateTime.Today,
            IsDirty = true 
        };
        
        _allJobs.Insert(0, newJob);
        ApplyFilter(); 
        SelectedJob = newJob;
    }

    [RelayCommand]
    private async Task SaveJob()
    {
        if (SelectedJob != null && !string.IsNullOrEmpty(CurrentDataPath))
        {
            SelectedJob.UpdatedDate = DateTime.Now;
            await _jobService.SaveJobAsync(CurrentDataPath, SelectedJob);
            SelectedJob.IsDirty = false;
            IsEditing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteJob()
    {
        if (SelectedJob != null && !string.IsNullOrEmpty(CurrentDataPath))
        {
            if (ConfirmDeleteAction != null)
            {
                bool confirm = await ConfirmDeleteAction.Invoke(SelectedJob.Company);
                if (!confirm) return;
            }

            _jobService.DeleteJob(CurrentDataPath, SelectedJob.Id);
            _allJobs.Remove(SelectedJob);
            ApplyFilter();
            SelectedJob = Jobs.FirstOrDefault();
        }
    }

    public async Task<bool> CanCloseAsync()
    {
        if (SelectedJob != null && SelectedJob.IsDirty)
        {
            if (ConfirmSaveAction != null)
            {
                var result = await ConfirmSaveAction.Invoke(SelectedJob.Company);

                if (result == ConfirmationResult.Cancel)
                {
                    return false;
                }
                else if (result == ConfirmationResult.Yes)
                {
                    if (!string.IsNullOrEmpty(CurrentDataPath))
                    {
                        await _jobService.SaveJobAsync(CurrentDataPath, SelectedJob);
                        SelectedJob.IsDirty = false;
                    }
                }
                // No means discard changes, which is fine for closing
            }
        }
        return true;
    }
}