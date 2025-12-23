using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WorkHammer.Models;

public class AppSettings
{
    public bool IsTransparencyEnabled { get; set; } = true;
    public string DataPath { get; set; } = string.Empty;
    public double WindowWidth { get; set; } = 1000;
    public double WindowHeight { get; set; } = 600;
    public string CurrentSort { get; set; } = "LastUpdate";
    public bool IsAscending { get; set; } = false;
}

public class SettingsService
{
    private readonly string _settingsFilePath;

    public SettingsService()
    {
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var rootDir = Path.Combine(homePath, "wh");
        
        if (!Directory.Exists(rootDir))
        {
            Directory.CreateDirectory(rootDir);
        }

        _settingsFilePath = Path.Combine(rootDir, "settings.json");
    }

    public async Task<AppSettings> LoadSettingsAsync()
    {
        if (!File.Exists(_settingsFilePath))
        {
            var defaultSettings = new AppSettings 
            { 
                DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "wh", "jobs") 
            };
            await SaveSettingsAsync(defaultSettings);
            return defaultSettings;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_settingsFilePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            
            // Ensure we have sane defaults if missing from JSON
            if (settings.WindowWidth <= 0) settings.WindowWidth = 1000;
            if (settings.WindowHeight <= 0) settings.WindowHeight = 600;
            
            return settings;
        }
        catch
        {
            return new AppSettings();
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_settingsFilePath, json);
    }

    public async Task SaveWindowSizeAsync(double width, double height)
    {
        var settings = await LoadSettingsAsync();
        settings.WindowWidth = width;
        settings.WindowHeight = height;
        await SaveSettingsAsync(settings);
    }
}
