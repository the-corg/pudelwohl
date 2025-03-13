using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public interface IServiceDataService
    {
        ObservableCollection<ServiceViewModel> Services { get; }
        ObservableCollection<ServiceBooking> ServiceBookings { get; }
        ListCollectionView ServiceBookingsForGuest { get; }
        ListCollectionView ServiceBookingsForService { get; }
        IMessageService MessageService { get; }
        Task LoadAsync();
        Task SaveDataAsync();
        void DebouncedSave();
    }
    public class ServiceDataService : BaseDataService, IServiceDataService
    {
        private readonly IServiceDataProvider _serviceDataProvider;
        private readonly IServiceBookingDataProvider _serviceBookingDataProvider;

        private readonly SemaphoreSlim _saveLock = new(1, 1);

        public ServiceDataService(IServiceDataProvider serviceDataProvider, 
            IServiceBookingDataProvider serviceBookingDataProvider, IMessageService messageService)
        {
            _serviceDataProvider = serviceDataProvider;
            _serviceBookingDataProvider = serviceBookingDataProvider;
            MessageService = messageService;
            ServiceBookingsForGuest = new ListCollectionView(ServiceBookings);
            ServiceBookingsForService = new ListCollectionView(ServiceBookings);
        }

        public ObservableCollection<ServiceViewModel> Services { get; } = new();
        public ObservableCollection<ServiceBooking> ServiceBookings { get; } = new();
        public ListCollectionView ServiceBookingsForGuest { get; }
        public ListCollectionView ServiceBookingsForService { get; }
        public IMessageService MessageService { get; }

        protected override CancellationTokenSource DebounceCts { get; set; } = new();

        public async Task LoadAsync()
        {
            var services = await _serviceDataProvider.GetAllAsync();
            LoadCollection(Services, services, service => new ServiceViewModel(service, this));

            var serviceBookings = await _serviceBookingDataProvider.GetAllAsync();
            LoadCollection(ServiceBookings, serviceBookings);
        }

        public override async Task SaveDataAsync()
        {
            // Prevent multiple saves from running at the same time
            await _saveLock.WaitAsync();
            try
            {
                // Cancel any pending debounced save
                DebounceCts.Cancel();

                // TODO await _guestDataProvider.SaveAsync(Guests.Select(x => x.GetGuest()));
            }
            finally
            {
                _saveLock.Release();
            }
        }

    }
}
