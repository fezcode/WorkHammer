using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using WorkHammer.ViewModels;

namespace WorkHammer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Wire up the confirmation dialogs when DataContext is set
        DataContextChanged += (s, e) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.ConfirmSaveAction = ShowSaveConfirmDialog;
                vm.ConfirmDeleteAction = ShowDeleteConfirmDialog;
                vm.ShowAboutAction = ShowAboutDialog;
            }
        };
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (_isClosing)
        {
            base.OnClosing(e);
            return;
        }

        if (DataContext is MainWindowViewModel vm)
        {
            e.Cancel = true;

            bool canClose = await vm.CanCloseAsync();
            if (canClose)
            {
                _isClosing = true;
                Close();
            }
        }
    }

    private bool _isClosing = false;

    private async void ShowAboutDialog()
    {
        var dialog = new ContentDialog
        {
            Title = "About WorkHammer",
            Content = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = "WorkHammer v1.0", FontWeight = Avalonia.Media.FontWeight.Bold, FontSize = 18 },
                    new TextBlock { Text = "A professional job application tracker for developers.", TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new TextBlock { Text = "Author: Fezcode (samil bulbul)", Opacity = 0.8 },
                    new Button 
                    { 
                        Content = "Homepage: https://fezcode.com", 
                        Foreground = Avalonia.Media.Brushes.SkyBlue,
                        BorderThickness = new Avalonia.Thickness(0),
                        Background = Avalonia.Media.Brushes.Transparent,
                        Padding = new Avalonia.Thickness(0),
                        Cursor = new Cursor(StandardCursorType.Hand)
                    }
                }
            },
            CloseButtonText = "Close"
        };

        var panel = (StackPanel)dialog.Content;
        var btn = (Button)panel.Children[3];
        btn.Click += (s, e) =>
        {
            Process.Start(new ProcessStartInfo("https://fezcode.com") { UseShellExecute = true });
        };

        await dialog.ShowAsync();
    }

    private void OnTitleBarPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    private void OnTitleBarDoubleTapped(object sender, TappedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized 
            ? WindowState.Normal 
            : WindowState.Maximized;
    }

    private async Task<bool> ShowDeleteConfirmDialog(string companyName)
    {
        var dialog = new ContentDialog
        {
            Title = "Confirm Delete",
            Content = $"Are you sure you want to delete {companyName}?\nThis action cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close
        };

        // Style the primary button as danger
        dialog.Opened += (s, e) =>
        {
            var btn = dialog.FindControl<Button>("PrimaryButton");
            if (btn != null)
            {
                btn.Classes.Add("danger");
            }
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    private async Task<ConfirmationResult> ShowSaveConfirmDialog(string companyName)
    {
        var dialog = new ContentDialog
        {
            Title = "Unsaved Changes",
            Content = $"Do you want to save changes to {companyName}?",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Don't Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };

        var result = await dialog.ShowAsync();

        return result switch
        {
            ContentDialogResult.Primary => ConfirmationResult.Yes,
            ContentDialogResult.Secondary => ConfirmationResult.No,
            _ => ConfirmationResult.Cancel
        };
    }

    private async void OnSelectFolderClicked(object? sender, RoutedEventArgs e)
    {
        var storage = this.StorageProvider;
        
        var folders = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Job Applications Folder",
            AllowMultiple = false
        });

        if (folders.Any())
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
