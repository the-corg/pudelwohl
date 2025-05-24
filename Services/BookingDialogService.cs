using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    /// <summary>
    /// Manages the Booking dialog 
    /// </summary>
    public interface IBookingDialogService
    {
        /// <summary>
        /// Creates and shows the dialog for booking a room.
        /// Also creates and sets up its view model.
        /// </summary>
        /// <param name="headerText">Text for the dialog header</param>
        /// <param name="isGuestSelectable">True if the guest should be selectable, false otherwise (guestId is fixed then)</param>
        /// <param name="isRoomSelectable">True if the room should be selectable, false otherwise (roomId is fixed then)</param>
        /// <param name="guestId">ID of the guest selected by default</param>
        /// <param name="roomId">ID of the room selected by default</param>
        /// <param name="booking">Booking to be edited (omit for adding a new one)</param>
        /// <returns>True, if the booking change or adding was confirmed<br/>False, otherwise</returns>
        bool ShowBookingDialog(string headerText, bool isGuestSelectable, bool isRoomSelectable, int guestId, int roomId, Booking? booking = null);
    }

    public class BookingDialogService : BaseDialogService, IBookingDialogService
    {
        #region Private fields and the constructor

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
        #endregion


        #region Public method (see interface)

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
        #endregion
    }
}
