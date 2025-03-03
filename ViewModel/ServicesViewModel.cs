using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class ServicesViewModel : ViewModelBase
    {
        private ServiceViewModel? _selectedService;
        private readonly IServiceDataService _serviceDataService;
        private readonly IMessageService _messageService;

        public ServicesViewModel(IServiceDataService serviceDataService, IMessageService messageService)
        {
            _serviceDataService = serviceDataService;
            _messageService = messageService;
            Services = serviceDataService.Services;
            AddCommand = new DelegateCommand(execute => Add());
            RemoveCommand = new DelegateCommand(execute => Remove(), canExecute => CanRemove());
        }

        public ObservableCollection<ServiceViewModel> Services { get; }

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
            }
        }

        // Used for hiding the service details when no service is selected
        public bool IsServiceSelected => SelectedService is not null;

        public DelegateCommand AddCommand { get; }

        public DelegateCommand RemoveCommand { get; }

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

    }
}
