using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using WorkHammer.ViewModels;

namespace WorkHammer.Views.Controls;

public partial class SettingsOverlayView : UserControl
{
    public SettingsOverlayView()
    {
        InitializeComponent();
    }

    private async void OnSelectFolderClicked(object? sender, RoutedEventArgs e)
    {
        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage == null) return;

        var folders = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Job Applications Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            var path = folders[0].Path.LocalPath;
            if (DataContext is MainWindowViewModel vm)
            {
                vm.CurrentDataPath = path;
                await vm.LoadJobsAsync();
            }
        }
    }
}
