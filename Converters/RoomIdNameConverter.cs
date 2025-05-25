using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    /// <summary>
    /// Converter that finds rooms by their ID
    /// </summary>
    class RoomIdNameConverter : IMultiValueConverter
    {
        /// <summary>
        /// Returns the room name based on its ID
        /// </summary>
        /// <param name="values">[0] is the ID of the room to find<br/>[1] is IRoomDataService that holds the collection of all rooms</param>
        /// <returns>Either the name of the room, or an error message, if there's no room with this ID</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // This check is needed to run the app in Debug mode
            if (values[0] == DependencyProperty.UnsetValue) return "Error: UnsetValue was sent to RoomIdNameConverter";
            var id = (int)(values[0]);
            var rooms = ((IRoomDataService)values[1]).Rooms;
            return rooms.FirstOrDefault(x => x.Id == id)?.Name ?? "Error: Room not found!";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
