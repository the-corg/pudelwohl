using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    /// <summary>
    /// Converter that finds guests by their ID
    /// </summary>
    class GuestIdConverter : IMultiValueConverter
    {
        /// <summary>
        /// Returns the requested attribute for a Guest based on the Guest's id
        /// </summary>
        /// <param name="values">[0] is the ID of the guest to find<br/>[1] is IGuestDataService that holds the collection of all guests</param>
        /// <param name="parameter">The attribute to find as a string ("Name" or "Breed")</param>
        /// <returns>Either the requested attribute of the guest, or an error message, if there's no guest with this ID</returns>
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
