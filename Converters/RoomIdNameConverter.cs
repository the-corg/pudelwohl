using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    class RoomIdNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // This check is needed to run the app in Debug mode
            if (values[0] == DependencyProperty.UnsetValue) return "ERROR";
            var id = (int)(values[0]);
            var rooms = ((GuestsViewModel) values[1]).Rooms;
            return rooms.First(x => x.Id == id).Name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
