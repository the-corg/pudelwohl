using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using System.Windows.Controls;
using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers
{
    // For adding the Add button to the collection of service bookings on the Services tab
    public class ServiceBookingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ServiceBookingTemplate { get; set; }
        public DataTemplate? AddButtonTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ServiceBooking serviceBooking && serviceBooking.ServiceId == -1)
                return AddButtonTemplate!;
            else
                return ServiceBookingTemplate!;
        }
    }
}
