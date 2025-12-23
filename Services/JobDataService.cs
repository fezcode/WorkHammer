using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WorkHammer.Models;

namespace WorkHammer.Services;

public class JobDataService
{
    public async Task<List<JobApplication>> LoadJobsAsync(string dataFolder)
    {
        var jobs = new List<JobApplication>();
        if (!Directory.Exists(dataFolder)) return jobs;

        var files = Directory.GetFiles(dataFolder, "*.json");
        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var job = JsonSerializer.Deserialize<JobApplication>(json);
                if (job != null)
                {
                    jobs.Add(job);
                }
            }
            catch (Exception) { }
        }
        return jobs.OrderByDescending(j => j.AppliedDate).ToList();
    }

    public async Task SaveJobAsync(string dataFolder, JobApplication job)
    {
        if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);
        var filePath = Path.Combine(dataFolder, $"{job.Id}.json");
        var json = JsonSerializer.Serialize(job, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public void DeleteJob(string dataFolder, Guid id)
    {
        var filePath = Path.Combine(dataFolder, $"{id}.json");
        if (File.Exists(filePath)) File.Delete(filePath);
    }

    public async Task<JobApplication?> LoadJobAsync(string dataFolder, Guid id)
    {
        var filePath = Path.Combine(dataFolder, $"{id}.json");
        if (!File.Exists(filePath)) return null;

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<JobApplication>(json);
        }
        catch { return null; }
    }
}
