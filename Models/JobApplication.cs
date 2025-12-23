using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkHammer.Models;

public enum JobStatus
{
    Applied,
    Interviewing,
    Offer,
    Rejected,
    Ghosted
}

public partial class JobResult : ObservableObject
{
    [ObservableProperty]
    private string _stage = string.Empty;

    [ObservableProperty]
    private string _reason = string.Empty;
}

public partial class JobLog : ObservableObject
{
    [ObservableProperty]
    private DateTime _date = DateTime.Now;

    [ObservableProperty]
    private string _stage = string.Empty;

    [ObservableProperty]
    private string _text = string.Empty;
}

public partial class JobApplication : ObservableObject
{
    public JobApplication()
    {
        TechStack.CollectionChanged += (s, e) => IsDirty = true;
        Stages.CollectionChanged += (s, e) => IsDirty = true;
        Logs.CollectionChanged += (s, e) => IsDirty = true;
        Result.PropertyChanged += Result_PropertyChanged;
    }

    private void Result_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        IsDirty = true;
    }

    partial void OnResultChanged(JobResult? oldValue, JobResult newValue)
    {
        if (oldValue != null) oldValue.PropertyChanged -= Result_PropertyChanged;
        newValue.PropertyChanged += Result_PropertyChanged;
        IsDirty = true;
    }

    [ObservableProperty]
    [property: JsonIgnore]
    private bool _isDirty;

    [ObservableProperty]
    private Guid _id = Guid.NewGuid();

    [ObservableProperty]
    private string _company = string.Empty;

    [ObservableProperty]
    private string _role = string.Empty;

    [ObservableProperty]
    private JobStatus _status = JobStatus.Applied;

    [ObservableProperty]
    private DateTime? _appliedDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _createdDate = DateTime.Now;

    [ObservableProperty]
    private DateTime? _updatedDate;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private string? _link;

    public ObservableCollection<string> TechStack { get; set; } = new();

    public ObservableCollection<string> Stages { get; set; } = new();

    public ObservableCollection<JobLog> Logs { get; set; } = new();

    [ObservableProperty]
    private JobResult _result = new();

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName != nameof(IsDirty) && e.PropertyName != nameof(TechStack) && e.PropertyName != nameof(Stages) && e.PropertyName != nameof(Result) && e.PropertyName != nameof(Logs))
        {
            IsDirty = true;
        }
    }
}