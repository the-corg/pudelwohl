using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    /// <summary>
    /// Manages the Service booking dialog 
    /// </summary>
    public interface IServiceBookingDialogService
    {
        /// <summary>
        /// Creates and shows the dialog for booking a service.
        /// Also creates and sets up its view model.
        /// </summary>
        /// <param name="isGuestSelectable">True if the guest should be selectable, false otherwise (<paramref name="guestId"/> is fixed then)</param>
        /// <param name="isServiceSelectable">True if the service should be selectable, false otherwise (<paramref name="serviceId"/> is fixed then)</param>
        /// <param name="isTimeSlotSelectable">True if the time slot should be selectable, false otherwise (<paramref name="startTime"/> is fixed then)</param>
        /// <param name="guestId">ID of the guest selected by default</param>
        /// <param name="serviceId">ID of the service selected by default</param>
        /// <param name="startTime">Start time of the time slot selected by default</param>
        /// <returns>True, if the new service booking was confirmed<br/>False, otherwise</returns>
        bool ShowServiceBookingDialog(bool isGuestSelectable, bool isServiceSelectable,
            bool isTimeSlotSelectable, int guestId, int serviceId, string? startTime);
    }

    public class ServiceBookingDialogService : BaseDialogService, IServiceBookingDialogService
    {
        #region Private fields and the constructor

        private readonly IGuestDataService _guestDataService;
        private readonly IServiceDataService _serviceDataService;
        private readonly IMessageService _messageService;

        public ServiceBookingDialogService(IGuestDataService guestDataService, 
            IServiceDataService serviceDataService, IMessageService messageService)
        {
            _guestDataService = guestDataService;
            _serviceDataService = serviceDataService;
            _messageService = messageService;
        }
        #endregion


        #region Public method (see interface)

        public bool ShowServiceBookingDialog(bool isGuestSelectable, bool isServiceSelectable,
            bool isTimeSlotSelectable, int guestId, int serviceId, string? startTime)
        {
            var dialog = new ServiceBookingDetails();
            var viewModel = new ServiceBookingDetailsViewModel(_guestDataService, _serviceDataService, _messageService,
                isGuestSelectable, isServiceSelectable, isTimeSlotSelectable, guestId, serviceId, startTime);
            dialog.DataContext = viewModel;
            viewModel.CloseOnConfirmAction = () => dialog.DialogResult = true;
            return ShowDialog(dialog);
        }
        #endregion
    }
}
