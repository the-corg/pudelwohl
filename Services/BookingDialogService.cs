using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    public interface IBookingDialogService
    {
        bool ShowBookingDialog(string headerText, bool isGuestSelectable, bool isRoomSelectable, int guestId, int roomId, Booking? booking = null);
    }

    public class BookingDialogService : IBookingDialogService
    {
        private readonly IGuestDataService _guestDataService;
        private readonly IRoomDataService _roomDataService;

        private Window? _mainWindow;
        private Window? MainWindow => _mainWindow ??= Application.Current.MainWindow; // Lazy initialization
        

        public BookingDialogService(IGuestDataService guestDataService, IRoomDataService roomDataService)
        {
            _guestDataService = guestDataService;
            _roomDataService = roomDataService;
        }

        public bool ShowBookingDialog(string headerText, bool isGuestSelectable, bool isRoomSelectable, int guestId, int roomId, Booking? booking = null)
        {
            var bookingDetails = new BookingDetails(_guestDataService, _roomDataService, headerText, isGuestSelectable, isRoomSelectable, guestId, roomId, booking);
            // Dim main window before showing the modal window, then restore it back
            if (MainWindow is not null) MainWindow.Opacity = 0.4;
            bool? result = bookingDetails.ShowDialog();
            if (MainWindow is not null) MainWindow.Opacity = 1.0;
            return result == true;
        }
    }
}
