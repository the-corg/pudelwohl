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

    public class BookingDialogService : BaseDialogService, IBookingDialogService
    {
        private readonly IGuestDataService _guestDataService;
        private readonly IRoomDataService _roomDataService;
        private readonly IMessageService _messageService;

        public BookingDialogService(IGuestDataService guestDataService, 
            IRoomDataService roomDataService, IMessageService messageService)
        {
            _guestDataService = guestDataService;
            _roomDataService = roomDataService;
            _messageService = messageService;
        }

        public bool ShowBookingDialog(string headerText, bool isGuestSelectable, bool isRoomSelectable, 
            int guestId, int roomId, Booking? booking = null)
        {
            var dialog = new BookingDetails();
            var viewModel = new BookingDetailsViewModel(_guestDataService, _roomDataService, _messageService,
                headerText, isGuestSelectable, isRoomSelectable, guestId, roomId, booking);
            dialog.DataContext = viewModel;
            viewModel.CloseOnConfirmAction = () => dialog.DialogResult = true;
            return ShowDialog(dialog);
        }
    }
}
