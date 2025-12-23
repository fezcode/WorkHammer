using System;
using System.Globalization;
using Avalonia.Data.Converters;
using WorkHammer.Models;

namespace WorkHammer.Views.Converters;

public class StatusToPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is JobStatus status)
        {
            return status switch
            {
                // Filled SVG paths for status icons
                JobStatus.Applied => "M20,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V6C22,4.89 21.1,4 20,4M20,8L12,13L4,8V6L12,11L20,6V8Z",
                JobStatus.Interviewing => "M16,13C15.71,13 15.38,13 15.03,13.05C16.19,13.89 17,15.22 17,16.5V19H22V16.5C22,14.67 18.33,13.75 16,13.75M9,13C6.67,13 3,13.92 3,16.25V19H15V16.25C15,13.92 11.33,13 9,13M9,4A4,4 0 0,0 5,8A4,4 0 0,0 9,12A4,4 0 0,0 13,8A4,4 0 0,0 9,4M16,4A3,3 0 0,0 13,7A3,3 0 0,0 16,10A3,3 0 0,0 19,7A3,3 0 0,0 16,4Z",
                JobStatus.Offer => "M12,17.27L18.18,21L16.54,13.97L22,9.24L14.81,8.62L12,2L9.19,8.62L2,9.24L7.45,13.97L5.82,21L12,17.27Z",
                JobStatus.Rejected => "M12,2C6.47,2 2,6.47 2,12C2,17.53 6.47,22 12,22C17.53,22 22,17.53 22,12C22,6.47 17.53,2 12,2M17,15.59L15.59,17L12,13.41L8.41,17L7,15.59L10.59,12L7,8.41L8.41,7L12,10.59L15.59,7L17,8.41L13.41,12L17,15.59Z",
                JobStatus.Ghosted => "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4M12,10A2,2 0 0,0 10,12A2,2 0 0,0 12,14A2,2 0 0,0 14,12A2,2 0 0,0 12,10Z",
                _ => ""
            };
        }
        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
