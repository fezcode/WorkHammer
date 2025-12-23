using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using WorkHammer.Models;

namespace WorkHammer.ViewModels;

public partial class MainWindowViewModel
{
    partial void OnIsTransparencyEnabledChanged(bool value) => _isSettingsDirty = true;
    
    partial void OnCurrentDataPathChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && Directory.Exists(value))
        {
            _ = LoadJobsAsync();
        }
    }

    [RelayCommand]
    private async Task ToggleSettings()
    {
        if (IsSettingsOpen)
        {
            if (_isSettingsDirty && ConfirmSaveAction != null)
            {
                var result = await ConfirmSaveAction.Invoke("App Settings");
                if (result == ConfirmationResult.Cancel) return;
                if (result == ConfirmationResult.Yes) await SaveAppSettingsAsync();
                else
                {
                    var settings = await _settingsService.LoadSettingsAsync();
                    IsTransparencyEnabled = settings.IsTransparencyEnabled;
                }
            }
            IsSettingsOpen = false;
            _isSettingsDirty = false;
        }
        else
        {
            IsSettingsOpen = true;
            IsEditing = false;
        }
    }

    private async Task SaveAppSettingsAsync()
    {
        var settings = await _settingsService.LoadSettingsAsync();
        settings.IsTransparencyEnabled = IsTransparencyEnabled;
        await _settingsService.SaveSettingsAsync(settings);
        _isSettingsDirty = false;
    }

    public async Task SaveWindowSizeAsync(double width, double height)
    {
        await _settingsService.SaveWindowSizeAsync(width, height);
    }

    [RelayCommand]
    private async Task SetAsDefaultDirectory()
    {
        if (string.IsNullOrEmpty(CurrentDataPath)) return;
        var settings = await _settingsService.LoadSettingsAsync();
        settings.DataPath = CurrentDataPath;
        await _settingsService.SaveSettingsAsync(settings);
        DefaultDataPath = CurrentDataPath;
    }

    public async Task<bool> CanCloseAsync()
    {
        if (_isSettingsDirty && ConfirmSaveAction != null)
        {
            var result = await ConfirmSaveAction.Invoke("App Settings");
            if (result == ConfirmationResult.Cancel) return false;
            if (result == ConfirmationResult.Yes) await SaveAppSettingsAsync();
        }

        if (SelectedJob != null && SelectedJob.IsDirty)
        {
            if (ConfirmSaveAction != null)
            {
                var result = await ConfirmSaveAction.Invoke(SelectedJob.Company);
                if (result == ConfirmationResult.Cancel) return false;
                else if (result == ConfirmationResult.Yes)
                {
                    if (!string.IsNullOrEmpty(CurrentDataPath))
                    {
                        await _jobService.SaveJobAsync(CurrentDataPath, SelectedJob);
                        SelectedJob.IsDirty = false;
                    }
                }
            }
        }
        return true;
    }
}
