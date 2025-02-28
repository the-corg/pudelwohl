using System.Globalization;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    class DateOnlyDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateOnly?)value)?.ToDateTime(TimeOnly.MinValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? null : DateOnly.FromDateTime(((DateTime)value));
        }
    }
}
