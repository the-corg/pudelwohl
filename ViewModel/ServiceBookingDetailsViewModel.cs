using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for the Service Booking Details dialog
    /// </summary>
    public class ServiceBookingDetailsViewModel : ViewModelBase
    {
        #region Private fields

        private readonly string _headerText;
        private DateOnly? _date;
        private readonly int _initialGuestId;
        private readonly int _initialServiceId;
        private readonly string? _initialStartTime;
        private string? _guestName;
        private string? _serviceName;
        private string? _timeSlot;
        private readonly IGuestDataService _guestDataService;
        private readonly IServiceDataService _serviceDataService;
        private readonly IMessageService _messageService;
        #endregion


        #region Constructor

        public ServiceBookingDetailsViewModel(IGuestDataService guestDataService, IServiceDataService serviceDataService,
            IMessageService messageService, bool isGuestSelectable, bool isServiceSelectable, bool isTimeSlotSelectable,
            int fixedGuestId, int fixedServiceId, string? fixedStartTime)
        {
            _guestDataService = guestDataService;
            _serviceDataService = serviceDataService;
            _messageService = messageService;
            _headerText = "New Service Booking";
            IsGuestSelectable = isGuestSelectable;
            IsServiceSelectable = isServiceSelectable;
            IsTimeSlotSelectable = isTimeSlotSelectable;

            _date = DateOnly.FromDateTime(DateTime.Now);
            _initialGuestId = fixedGuestId;
            _initialServiceId = fixedServiceId;
            _initialStartTime = fixedStartTime;

            InitializeNames();
            ConfirmCommand = new DelegateCommand(async execute => await Confirm(), canExecute => CanConfirm());
        }
        #endregion


        #region Public properties

        /// <summary>
        /// Delegate for closing the dialog window on Confirm
        /// </summary>
        public Action? CloseOnConfirmAction { get; set; }

        /// <summary>
        /// Header text for the dialog
        /// </summary>
        public string HeaderText => _headerText;

        /// <summary>
        /// Date of the service booking
        /// </summary>
        public DateOnly? Date
        {
            get => _date;
            set
            {
                if (value == _date)
                    return;

                _date = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Shows whether the guest is selectable
        /// (if false, it's already pre-selected)
        /// </summary>
        public bool IsGuestSelectable { get; }

        /// <summary>
        /// Name of the guest
        /// </summary>
        public string? GuestName
        {
            get => _guestName;
            set
            {
                if (value == _guestName)
                    return;
                _guestName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Shows whether the service is selectable
        /// (if false, it's already pre-selected)
        /// </summary>
        public bool IsServiceSelectable { get; }

        /// <summary>
        /// Name of the service
        /// </summary>
        public string? ServiceName
        {
            get => _serviceName;
            set
            {
                if (value == _serviceName)
                    return;
                _serviceName = value;
                ResetTimeSlots();
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Shows whether the time slot is selectable
        /// (if false, it's already pre-selected)
        /// </summary>
        public bool IsTimeSlotSelectable { get; }

        /// <summary>
        /// String representation of the time slot
        /// </summary>
        public string? TimeSlot
        {
            get => _timeSlot;
            set
            {
                if (value == _timeSlot)
                    return;
                _timeSlot = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Names of available guests
        /// </summary>
        public List<string> GuestNames { get; } = new();

        /// <summary>
        /// Names of available services
        /// </summary>
        public List<string> ServiceNames { get; } = new();

        /// <summary>
        /// String representations of available time slots
        /// </summary>
        public ObservableCollection<string> TimeSlots { get; } = new();

        /// <summary>
        /// Command to confirm the service booking
        /// </summary>
        public DelegateCommand ConfirmCommand { get; }

        /// <summary>
        /// Text that explains the reason why the Confirm button is disabled
        /// </summary>
        public string? ButtonDisabledReason
        {
            get
            {
                string result = "";

                if (Date is null)
                    result += "Please select a date.\n\n";
                else if (Date < DateOnly.FromDateTime(DateTime.Now))
                    result += "Please select a date that is not in the past.\n\n";

                if (TimeSlot is null)
                    result += "Please select a time slot.\n\n";
                if (GuestName is null)
                    result += "Please select a guest.\n\n";
                if (ServiceName is null)
                    result += "Please select a service.\n\n";

                return result.Length > 0 ? result[..^2] : null;
            }
        }
        #endregion


        #region Private helper methods

        // Make the Confirm button inactive if some info is missing or the date is in the past
        private bool CanConfirm() => !(Date is null || GuestName is null ||
            ServiceName is null || TimeSlot is null || Date < DateOnly.FromDateTime(DateTime.Now));
        private async Task Confirm()
        {
            // Race condition check - UI is not guaranteed to check CanConfirm immediately before Confirm
            if (!CanConfirm())
            {
                ConfirmCommand.OnCanExecuteChanged();
                return;
            }

            // If guest was selecteble, parse GuestName
            int guestId = IsGuestSelectable ? int.Parse(GuestName!.Split("#").Last()[..^1]) : _initialGuestId;

            // If service was selectable, parse ServiceName
            int serviceId = IsServiceSelectable ? int.Parse(ServiceName!.Split("#").Last()[..^1]) : _initialServiceId;

            // If time slot was selectable, get start time
            var timeSlotSplit = TimeSlot!.Split();
            if (timeSlotSplit.Length != 3)
            {
                _messageService.ShowMessage("Time slot error in Service Booking Details.\nContact the developers.");
                return;
            }
            var startTimeString = timeSlotSplit[0];
            TimeOnly startTime = TimeOnly.Parse(timeSlotSplit[0]);
            TimeOnly endTime = TimeOnly.Parse(timeSlotSplit[2]);

            // Check overlapping
            foreach (var sBooking in _serviceDataService.ServiceBookings)
            {
                if (sBooking.Date != _date)
                    continue;

                var serviceToCheck = _serviceDataService.Services.FirstOrDefault(x => x.Id == sBooking.ServiceId);
                if (serviceToCheck is null)
                    continue;

                var sBookingStartTime = TimeOnly.Parse(sBooking.StartTime!);
                var sBookingEndTime = sBookingStartTime.AddMinutes(serviceToCheck.DurationMinutes);

                // Check if the guest has an overlapping booked time slot
                if (sBooking.GuestId == guestId && sBookingStartTime < endTime && sBookingEndTime > startTime)
                {
                    _messageService.ShowMessage("This time slot overlaps with another service booked for this guest!");
                    return;
                }
                // Check if the service has an overlapping booked time slot
                if (sBooking.ServiceId == serviceId && sBookingStartTime < endTime && sBookingEndTime > startTime)
                {
                    _messageService.ShowMessage("This time slot overlaps with another time slot booked for this service!");
                    return;
                }
            }

            // All checks done. Add the serviceBooking now
            var serviceBooking = new ServiceBooking()
            {
                GuestId = guestId,
                ServiceId = serviceId,
                Date = (DateOnly)_date!,
                StartTime = startTimeString
            };

            _serviceDataService.ServiceBookings.Add(serviceBooking);
            
            // Close the dialog
            CloseOnConfirmAction?.Invoke();

            await _serviceDataService.SaveDataAsync();
        }

        private void ResetTimeSlots()
        {
            TimeSlots.Clear();
            if (ServiceName is null)
                return;

            int serviceId = _initialServiceId == -1 ? int.Parse(ServiceName!.Split("#").Last()[..^1]) : _initialServiceId;

            var service = _serviceDataService.Services.FirstOrDefault(x => x.Id == serviceId);
            if (service is null)
                return;

            foreach (var timeSlot in service.TimeSlots)
                TimeSlots.Add(timeSlot.ToString());
        }

        private void InitializeNames()
        {
            GuestNames.AddRange(_guestDataService.Guests.Select(x => x.ToString()));

            ServiceNames.AddRange(_serviceDataService.Services.Select(x => x.ToString()));

            if (_initialGuestId != -1)
            {
                _guestName = _guestDataService.Guests.FirstOrDefault(x => x.Id == _initialGuestId)?.ToString();
            }

            if (_initialServiceId != -1)
            {
                _serviceName = _serviceDataService.Services.FirstOrDefault(x => x.Id == _initialServiceId)?.ToString();
                ResetTimeSlots();

                if (_initialStartTime is not null)
                {
                    _timeSlot = TimeSlots.FirstOrDefault(x => x.Split()[0] == _initialStartTime);
                }
            }
        }
        #endregion

    }
}
