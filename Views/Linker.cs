using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace WorkHammer.Views;

public static class Linker
{
    private static readonly Regex UrlRegex = new Regex(@"https?://[^\s/$.?#].[^\s]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static readonly AttachedProperty<string?> TextProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, string?>("Text", typeof(Linker));

    public static string? GetText(TextBlock element) => element.GetValue(TextProperty);
    public static void SetText(TextBlock element, string? value) => element.SetValue(TextProperty, value);

    static Linker()
    {
        TextProperty.Changed.AddClassHandler<TextBlock>((tb, e) => UpdateInlines(tb, e.NewValue as string));
    }

    private static void UpdateInlines(TextBlock tb, string? text)
    {
        tb.Inlines?.Clear();
        if (string.IsNullOrEmpty(text)) return;

        var lastIndex = 0;
        foreach (Match match in UrlRegex.Matches(text))
        {
            // Add text before the URL
            if (match.Index > lastIndex)
            {
                tb.Inlines?.Add(new Run { Text = text.Substring(lastIndex, match.Index - lastIndex) });
            }

            // Add the URL as a link
            var url = match.Value;
            
            var btn = new Button
            {
                Content = url,
                Padding = new Thickness(0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.SkyBlue,
                FontSize = tb.FontSize,
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };
            btn.Classes.Add("toolbar-link");
            btn.Click += (s, e) => 
            {
                try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
            };

            tb.Inlines?.Add(new InlineUIContainer(btn));
            lastIndex = match.Index + match.Length;
        }

        // Add remaining text
        if (lastIndex < text.Length)
        {
            tb.Inlines?.Add(new Run { Text = text.Substring(lastIndex) });
        }
    }
}
