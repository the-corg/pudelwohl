using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using System.Windows.Controls;
using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers
{
    // For adding the AddBooking button to the collection of bookings on the Rooms tab
    public class BookingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BookingTemplate { get; set; }
        public DataTemplate? AddButtonTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Booking booking && booking.RoomId == -1)
                return AddButtonTemplate!;
            else
                return BookingTemplate!;
        }
    }
}
