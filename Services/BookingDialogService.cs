using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

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
            var dialogWindow = new BookingDetails();
            var viewModel = new BookingDetailsViewModel(_guestDataService, _roomDataService, headerText, 
                isGuestSelectable, isRoomSelectable, guestId, roomId, booking);
            dialogWindow.DataContext = viewModel;
            viewModel.CloseOnConfirmAction = () => dialogWindow.DialogResult = true;

            // Dim main window before showing the modal window, then restore it back
            if (MainWindow is not null) MainWindow.Opacity = 0.4;
            bool? result = dialogWindow.ShowDialog();
            if (MainWindow is not null) MainWindow.Opacity = 1.0;
            return result == true;
        }
    }
}
