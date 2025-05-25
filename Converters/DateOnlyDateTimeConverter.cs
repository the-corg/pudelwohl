using System.Globalization;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    /// <summary>
    /// Converts between DateOnly and DateTime
    /// </summary>
    class DateOnlyDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a DateOnly struct provided in <paramref name="value"/> to DateTime
        /// </summary>
        /// <param name="value">DateOnly struct to convert to DateTime</param>
        /// <returns>DateTime struct obtained by converting the DateOnly struct provided in <paramref name="value"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateOnly?)value)?.ToDateTime(TimeOnly.MinValue);
        }

        /// <summary>
        /// Converts a DateTime struct provided in <paramref name="value"/> to DateOnly
        /// </summary>
        /// <param name="value">DateTime struct to convert to DateOnly</param>
        /// <returns>DateOnly struct obtained by converting the DateTime struct provided in <paramref name="value"/></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? null : DateOnly.FromDateTime(((DateTime)value));
        }
    }
}
