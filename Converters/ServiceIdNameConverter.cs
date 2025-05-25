using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    /// <summary>
    /// Converter that finds services by their ID
    /// </summary>
    class ServiceIdNameConverter : IMultiValueConverter
    {
        /// <summary>
        /// Returns the service name based on its ID
        /// </summary>
        /// <param name="values">[0] is the ID of the service to find<br/>[1] is IServiceDataService that holds the collection of all services</param>
        /// <returns>Either the name of the service, or an error message, if there's no service with this ID</returns>
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
