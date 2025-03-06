using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    class GuestIdConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is null) return "nobody";
            // This check is needed to run the app in Debug mode
            if (values[0] == DependencyProperty.UnsetValue) return "Error: UnsetValue was sent to GuestIdNameConverter";
            var id = (int)(values[0]);
            var guests = ((IGuestDataService)values[1]).Guests;
            var guest = guests.FirstOrDefault(x => x.Id == id);
            if ((string)parameter == "Name")
                return guest?.Name ?? "Error: Guest not found!";
            else if ((string)parameter == "Breed")
                return guest?.Breed ?? "Error: Guest not found!";
            else
                return "GuestIdConverter: Unknown parameter";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
