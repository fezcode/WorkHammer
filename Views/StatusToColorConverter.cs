using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using WorkHammer.Models;

namespace WorkHammer.Views.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is JobStatus status)
        {
            return status switch
            {
                JobStatus.Applied => Brush.Parse("#2b88d8"),
                JobStatus.Interviewing => Brush.Parse("#9b59b6"),
                JobStatus.Offer => Brush.Parse("#2ecc71"),
                JobStatus.Rejected => Brush.Parse("#e74c3c"),
                JobStatus.Ghosted => Brush.Parse("#95a5a6"),
                _ => Brush.Parse("#666666")
            };
        }
        return Brush.Parse("#666666");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
