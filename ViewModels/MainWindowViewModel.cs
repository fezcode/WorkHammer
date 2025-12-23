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
public enum JobSortOption { Name, Status, DateApplied, LastUpdate, CreatedDate }

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly JobDataService _jobService;
    private readonly SettingsService _settingsService;
    private bool _isRestoringSelection; 
    private List<JobApplication> _allJobs = new();
    private bool _isSettingsDirty;
    private bool _isCheckingDirty;

    [ObservableProperty] private string? _currentDataPath;
    [ObservableProperty] private string? _defaultDataPath;
    [ObservableProperty] private ObservableCollection<JobApplication> _jobs = new();
    [ObservableProperty] private JobApplication? _selectedJob;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private JobStatus? _statusFilter;
    [ObservableProperty] private JobSortOption _currentSort = JobSortOption.LastUpdate;
    [ObservableProperty] private bool _isAscending = false;
    [ObservableProperty] private string _statsText = "No applications loaded";
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private bool _isSettingsOpen;
    [ObservableProperty] private bool _isTransparencyEnabled = true;
    [ObservableProperty] private double _windowWidth = 1000;
    [ObservableProperty] private double _windowHeight = 600;

    [ObservableProperty] private string _newTechTag = string.Empty;
    [ObservableProperty] private string _newStageName = string.Empty;
    [ObservableProperty] private string _newLogText = string.Empty;
    [ObservableProperty] private string _newLogStage = string.Empty;
    [ObservableProperty] private DateTime? _newLogDate = DateTime.Today;

    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _appliedCount;
    [ObservableProperty] private int _interviewingCount;
    [ObservableProperty] private int _offersCount;
    [ObservableProperty] private int _rejectedCount;
    [ObservableProperty] private int _ghostedCount;

    public JobStatus[] JobStatuses => Enum.GetValues<JobStatus>();

    // Sorting Helpers
    public bool IsNameSortActive => CurrentSort == JobSortOption.Name;
    public bool IsStatusSortActive => CurrentSort == JobSortOption.Status;
    public bool IsDateSortActive => CurrentSort == JobSortOption.DateApplied;
    public bool IsUpdateSortActive => CurrentSort == JobSortOption.LastUpdate;
    public bool IsCreatedSortActive => CurrentSort == JobSortOption.CreatedDate;
    public string SortDirectionIcon => IsAscending ? "M7 14l5-5 5 5H7z" : "M7 10l5 5 5-5H7z";

    // Callbacks to the View
    public Func<string, Task<ConfirmationResult>>? ConfirmSaveAction { get; set; }
    public Func<string, Task<bool>>? ConfirmDeleteAction { get; set; }
    public Action? ShowAboutAction { get; set; }

    public MainWindowViewModel()
    {
        _jobService = new JobDataService();
        _settingsService = new SettingsService();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        var settings = await _settingsService.LoadSettingsAsync();
        
        IsTransparencyEnabled = settings.IsTransparencyEnabled;
        CurrentDataPath = settings.DataPath;
        DefaultDataPath = settings.DataPath;
        WindowWidth = settings.WindowWidth;
        WindowHeight = settings.WindowHeight;

        if (!string.IsNullOrEmpty(CurrentDataPath) && !Directory.Exists(CurrentDataPath))
        {
            try { Directory.CreateDirectory(CurrentDataPath); } catch { }
        }

        if (!string.IsNullOrEmpty(CurrentDataPath) && Directory.Exists(CurrentDataPath))
        {
            await LoadJobsAsync();
        }

        _isSettingsDirty = false;
    }

    [RelayCommand]
    private void ShowAbout() => ShowAboutAction?.Invoke();
}