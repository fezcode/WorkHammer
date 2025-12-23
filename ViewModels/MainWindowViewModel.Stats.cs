using System.Linq;
using WorkHammer.Models;

namespace WorkHammer.ViewModels;

public partial class MainWindowViewModel
{
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
}
