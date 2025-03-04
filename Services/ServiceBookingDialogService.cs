using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    public interface IServiceBookingDialogService
    {
        bool ShowServiceBookingDialog(bool isGuestSelectable, bool isServiceSelectable,
            bool isTimeSlotSelectable, int guestId, int serviceId, string? startTime);
    }

    public class ServiceBookingDialogService : BaseDialogService, IServiceBookingDialogService
    {
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
    }
}
