using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using System.ComponentModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class ServicesViewModel : ViewModelBase
    {
        private ServiceViewModel? _selectedService;
        private TimeSlot? _selectedTimeSlot;
        private readonly IServiceDataService _serviceDataService;
        private readonly IMessageService _messageService;

        public ServicesViewModel(IServiceDataService serviceDataService, IMessageService messageService)
        {
            _serviceDataService = serviceDataService;
            _messageService = messageService;
            Services = serviceDataService.Services;
            AddCommand = new DelegateCommand(execute => Add());
            RemoveCommand = new DelegateCommand(execute => Remove(), canExecute => CanRemove());
            BookServiceCommand = new DelegateCommand(execute => BookService(), canExecute => CanBookService());
            AddServiceBookingCommand = new DelegateCommand(execute => AddServiceBooking());

            ServiceBookingsCollectionView = serviceDataService.ServiceBookingsForService;
            // Filter service bookings based on the selected service
            ServiceBookingsCollectionView.Filter = 
                serviceBooking => (SelectedService is not null) && (((ServiceBooking)serviceBooking).ServiceId == SelectedService.Id);
            // And sort it by date, then by start time
            ServiceBookingsCollectionView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            ServiceBookingsCollectionView.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));

            // Composite collection to show on the Services tab, with an Add button after the bookings
            ServiceBookingsCompositeCollection = new CompositeCollection
            {
                    new CollectionContainer { Collection = ServiceBookingsCollectionView },
                    new ServiceBooking { ServiceId = -1 } // Fake item for the Add button
            };
        }

        public ObservableCollection<ServiceViewModel> Services { get; }
        public ListCollectionView ServiceBookingsCollectionView { get; }
        public CompositeCollection ServiceBookingsCompositeCollection { get; }

        public ServiceViewModel? SelectedService
        {
            get => _selectedService;
            set
            {
                if (_selectedService == value)
                    return;

                _selectedService = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsServiceSelected));
                RemoveCommand.OnCanExecuteChanged();
                ServiceBookingsCollectionView.Refresh();
            }
        }

        // Used for hiding the service details when no service is selected
        public bool IsServiceSelected => SelectedService is not null;

        public TimeSlot? SelectedTimeSlot
        {
            get => _selectedTimeSlot;
            set
            {
                if (_selectedTimeSlot == value)
                    return;

                _selectedTimeSlot = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTimeSlotSelected));
                OnPropertyChanged(nameof(GuestForSelectedTimeSlot));
                BookServiceCommand.OnCanExecuteChanged();
            }
        }
        public bool IsTimeSlotSelected => SelectedTimeSlot is not null;

        public string? GuestForSelectedTimeSlot
        {
            get
            {
                if (SelectedService is null || SelectedTimeSlot is null)
                    return null;
                return "TEST!!";
            }
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand RemoveCommand { get; }
        public DelegateCommand BookServiceCommand { get; }
        public DelegateCommand AddServiceBookingCommand { get; }

        private void Add()
        {
            var service = new Service { Name = "NEW SERVICE" };
            var viewModel = new ServiceViewModel(service, _serviceDataService);
            Services.Add(viewModel);
            SelectedService = viewModel;
        }

        private bool CanRemove() => SelectedService is not null;
        private void Remove()
        {
            if (SelectedService is null)
                return;

            // Count all bookings for the selected service and ask the user to confirm deletion
            var serviceBookings = _serviceDataService.ServiceBookings.Where(x => x.ServiceId == SelectedService.Id).ToList();
            int numberOfServiceBookings = serviceBookings.Count;

            if (numberOfServiceBookings > 0)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                int numberOfFutureServiceBookings = serviceBookings.Where(x => x.Date >= today).Count();

                string message = $"This service has {numberOfServiceBookings} booking" + (numberOfServiceBookings != 1 ? "s" : "") +
                    $", including {numberOfFutureServiceBookings} active booking" + (numberOfFutureServiceBookings != 1 ? "s" : "") +
                    $".\n\nAre you sure you want to delete the service and cancel all its bookings for the affected guests?";

                if (!_messageService.ShowConfirmation(message))
                    return;

                foreach (var serviceBooking in serviceBookings)
                    _serviceDataService.ServiceBookings.Remove(serviceBooking);
            }

            Services.Remove(SelectedService);
            SelectedService = null;
        }

        // For booking with a fixed time slot
        private bool CanBookService() => SelectedService is not null && SelectedTimeSlot is not null;
        private void BookService()
        {
            if (!CanBookService())
                return;
            SelectedTimeSlot = null;
        }

        // For booking without a fixed time slot
        private void AddServiceBooking()
        {
            if (SelectedService is null)
                return;

            //_bookingDialogService.ShowBookingDialog("New Booking", true, false, -1, SelectedRoom.Id);
        }
    }
}
