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

public partial class JobApplication : ObservableObject
{
    public JobApplication()
    {
        TechStack.CollectionChanged += (s, e) => IsDirty = true;
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
    private DateTime? _updatedDate;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private string? _link;

    public ObservableCollection<string> TechStack { get; set; } = new();

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName != nameof(IsDirty) && e.PropertyName != nameof(TechStack))
        {
            IsDirty = true;
        }
    }
}