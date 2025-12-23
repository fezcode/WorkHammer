using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Controls;

namespace WorkHammer.Views.Converters;

public class TransparencyConverter : IValueConverter
{
    public static readonly TransparencyConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool enabled && enabled)
        {
            return new WindowTransparencyLevel[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur };
        }
        return new WindowTransparencyLevel[] { WindowTransparencyLevel.None };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TransparencyBackgroundConverter : IValueConverter
{
    public static readonly TransparencyBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool enabled && enabled)
        {
            return Brushes.Transparent;
        }
        return Brush.Parse("#1a1a1a");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
