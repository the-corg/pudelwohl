using System.Globalization;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    /// <summary>
    /// Converter that determines if the provided date is in the past
    /// </summary>
    class IsDateInThePastConverter : IValueConverter
    {
        /// <summary>
        /// Determines if the date provided in <paramref name="value"/> is in the past
        /// </summary>
        /// <param name="value">Date as DateOnly</param>
        /// <returns>True, if the date provided in <paramref name="value"/> is in the past<br/>False, otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateOnly?)value) < DateOnly.FromDateTime(DateTime.Today);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
