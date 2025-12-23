using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace WorkHammer.Views.Converters;

public class PathTruncateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrEmpty(path))
        {
            int maxLength = 35; // Default max length
            if (parameter is string p && int.TryParse(p, out int m))
            {
                maxLength = m;
            }

            if (path.Length <= maxLength) return path;

            // Simple middle truncation
            string fileName = Path.GetFileName(path);
            string root = Path.GetPathRoot(path) ?? "";
            
            int reserved = fileName.Length + root.Length + 5; // +5 for ellipses and slashes
            int remaining = maxLength - reserved;

            if (remaining > 0)
            {
                string dir = Path.GetDirectoryName(path) ?? "";
                string mid = dir.Length > remaining ? "..." : dir;
                return Path.Combine(root, mid, fileName);
            }
            
            // Fallback: Start...End
            return path.Substring(0, maxLength / 2) + "..." + path.Substring(path.Length - (maxLength / 2));
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
