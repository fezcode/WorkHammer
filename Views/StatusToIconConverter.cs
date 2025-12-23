using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;
using FluentAvalonia.UI.Controls;
using WorkHammer.Models;

namespace WorkHammer.Views.Converters;

public class StatusToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is JobStatus status)
        {
            return status switch
            {
                JobStatus.Applied => Symbol.Send,
                JobStatus.Interviewing => Symbol.People,
                JobStatus.Offer => Symbol.Star,
                JobStatus.Rejected => Symbol.Dismiss,
                JobStatus.Ghosted => Symbol.Clear,
                _ => Symbol.Help
            };
        }
        return Symbol.Help;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
