using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WorkHammer.Views.Converters;

public class MathMultiplyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d && parameter is string p && double.TryParse(p, NumberStyles.Any, CultureInfo.InvariantCulture, out double factor))
        {
            return d * factor;
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
