using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataServices;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    class ServiceIdNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // This check is needed to run the app in Debug mode
            if (values[0] == DependencyProperty.UnsetValue) return "Error: UnsetValue was sent to ServiceIdNameConverter";
            var id = (int)(values[0]);
            var services = ((IServiceDataService)values[1]).Services;
            return services.FirstOrDefault(x => x.Id == id)?.Name ?? "Error: Service not found!";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
