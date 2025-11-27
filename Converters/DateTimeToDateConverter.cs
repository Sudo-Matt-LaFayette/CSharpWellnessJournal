using System;
using System.Globalization;
using System.Windows.Data;

namespace DailyCheckInJournal.Converters
{
    public class DateTimeToDateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.Date;
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date;
            }
            return DateTime.Now;
        }
    }
}

