using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkHammer.Models;

namespace WorkHammer.ViewModels;

public partial class MainWindowViewModel
{
    [RelayCommand]
    public async Task LoadJobsAsync()
    {
        if (string.IsNullOrEmpty(CurrentDataPath)) return;
        _allJobs = await _jobService.LoadJobsAsync(CurrentDataPath);
        foreach (var job in _allJobs) job.IsDirty = false;
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
        IsEditing = true;
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
            SelectedJob.IsDirty = false;
            var originalJob = await _jobService.LoadJobAsync(CurrentDataPath, SelectedJob.Id);
            if (originalJob != null)
            {
                originalJob.IsDirty = false;
                int index = _allJobs.FindIndex(j => j.Id == SelectedJob.Id);
                if (index != -1)
                {
                    _allJobs[index] = originalJob;
                    _isRestoringSelection = true;
                    ApplyFilter();
                    SelectedJob = originalJob;
                    _isRestoringSelection = false;
                }
            }
            else
            {
                _allJobs.Remove(SelectedJob);
                _isRestoringSelection = true;
                ApplyFilter();
                SelectedJob = Jobs.FirstOrDefault();
                _isRestoringSelection = false;
            }
        }
        IsEditing = false;
    }

    async partial void OnSelectedJobChanged(JobApplication? oldValue, JobApplication? newValue)
    {
        if (_isRestoringSelection || _isCheckingDirty) return;

        if (oldValue != null && oldValue.IsDirty)
        {
            _isCheckingDirty = true;
            _isRestoringSelection = true;
            var tempNewValue = newValue;
            SelectedJob = oldValue;
            _isRestoringSelection = false;

            if (ConfirmSaveAction != null)
            {
                var result = await ConfirmSaveAction.Invoke(oldValue.Company);
                if (result == ConfirmationResult.Cancel)
                {
                    _isCheckingDirty = false;
                    return;
                }

                _isRestoringSelection = true;
                if (result == ConfirmationResult.Yes)
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
                SelectedJob = tempNewValue;
                IsEditing = false;
                _isRestoringSelection = false;
            }
            _isCheckingDirty = false;
        }
        else
        {
            IsEditing = false;
        }

        if (oldValue != null) oldValue.PropertyChanged -= Job_PropertyChanged;
        if (newValue != null) newValue.PropertyChanged += Job_PropertyChanged;
    }

    private void Job_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(JobApplication.Status)) UpdateStats();
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
        IEnumerable<JobApplication> sorted = CurrentSort switch
        {
            JobSortOption.Name => IsAscending ? results.OrderBy(j => j.Company) : results.OrderByDescending(j => j.Company),
            JobSortOption.Status => IsAscending ? results.OrderBy(j => j.Status) : results.OrderByDescending(j => j.Status),
            JobSortOption.DateApplied => IsAscending ? results.OrderBy(j => j.AppliedDate) : results.OrderByDescending(j => j.AppliedDate),
            JobSortOption.LastUpdate => IsAscending ? results.OrderBy(j => j.UpdatedDate) : results.OrderByDescending(j => j.UpdatedDate),
            JobSortOption.CreatedDate => IsAscending ? results.OrderBy(j => j.CreatedDate) : results.OrderByDescending(j => j.CreatedDate),
            _ => results
        };
        Jobs = new ObservableCollection<JobApplication>(sorted);
        UpdateStats();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnStatusFilterChanged(JobStatus? value) => ApplyFilter();
    partial void OnCurrentSortChanged(JobSortOption value) => ApplyFilter();
    partial void OnIsAscendingChanged(bool value) => ApplyFilter();

    [RelayCommand]
    private void SetSort(JobSortOption option)
    {
        if (CurrentSort == option) IsAscending = !IsAscending;
        else
        {
            CurrentSort = option;
            IsAscending = (option == JobSortOption.Name);
        }
        OnPropertyChanged(nameof(IsNameSortActive));
        OnPropertyChanged(nameof(IsStatusSortActive));
        OnPropertyChanged(nameof(IsDateSortActive));
        OnPropertyChanged(nameof(IsUpdateSortActive));
        OnPropertyChanged(nameof(IsCreatedSortActive));
        OnPropertyChanged(nameof(SortDirectionIcon));
    }

    [RelayCommand]
    private void AddTechTag()
    {
        if (!string.IsNullOrWhiteSpace(NewTechTag) && SelectedJob != null)
        {
            var tag = NewTechTag.Trim();
            if (!SelectedJob.TechStack.Contains(tag)) SelectedJob.TechStack.Add(tag);
            NewTechTag = string.Empty;
        }
    }

    [RelayCommand] private void RemoveTechTag(string tag) => SelectedJob?.TechStack.Remove(tag);

    [RelayCommand]
    private void AddStage()
    {
        if (!string.IsNullOrWhiteSpace(NewStageName) && SelectedJob != null)
        {
            SelectedJob.Stages.Add(NewStageName.Trim());
            NewStageName = string.Empty;
        }
    }

    [RelayCommand] private void RemoveStage(string stage) => SelectedJob?.Stages.Remove(stage);

                            [RelayCommand]

                            private void AddLog()

                            {

                                if (!string.IsNullOrWhiteSpace(NewLogText) && SelectedJob != null)

                                {

                                    SelectedJob.Logs.Add(new JobLog

                                    {

                                        Date = NewLogDate ?? DateTime.Now,

                                        Stage = NewLogStage?.Trim() ?? string.Empty,

                                        Text = NewLogText.Trim()

                                    });

                                    NewLogText = string.Empty;

                                    NewLogStage = string.Empty;

                                    NewLogDate = DateTime.Today;

                                }

                            }

    [RelayCommand] private void RemoveLog(JobLog log) => SelectedJob?.Logs.Remove(log);

    [RelayCommand] private void ClearFilter() => StatusFilter = null;

    [RelayCommand]
    private void OpenUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        try
        {
            if (!url.StartsWith("http")) url = "https://" + url;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch { }
    }

    [RelayCommand]
    private void OpenExplorer()
    {
        if (!string.IsNullOrEmpty(CurrentDataPath) && Directory.Exists(CurrentDataPath))
            System.Diagnostics.Process.Start("explorer", CurrentDataPath);
    }
}
