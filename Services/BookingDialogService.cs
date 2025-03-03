using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    public interface IBookingDialogService
    {
        bool ShowBookingDialog(IRoomDataService roomDataService, string headerText, int guestId, int roomId, Booking? booking);
    }

    public class BookingDialogService : IBookingDialogService
    {
        public bool ShowBookingDialog(IRoomDataService roomDataService, string headerText, int guestId, int roomId, Booking? booking)
        {
            throw new NotImplementedException();
        }
    }
}
