using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    /// <summary>
    /// Manages all data related to services
    /// </summary>
    public interface IServiceDataService
    {
        /// <summary>
        /// The collection of services
        /// </summary>
        ObservableCollection<ServiceViewModel> Services { get; }

        /// <summary>
        /// The collection of service bookings
        /// </summary>
        ObservableCollection<ServiceBooking> ServiceBookings { get; }

        /// <summary>
        /// Sorted and filtered view for the collection of service bookings, with service bookings for only one guest
        /// </summary>
        ListCollectionView ServiceBookingsForGuest { get; }

        /// <summary>
        /// Sorted and filtered view for the collection of service bookings, with service bookings for only one service
        /// </summary>
        ListCollectionView ServiceBookingsForService { get; }

        /// <summary>
        /// Reference to the message service (used in ServiceViewModel,
        /// for the messages related to validation after service data change)
        /// </summary>
        IMessageService MessageService { get; }

        /// <summary>
        /// Loads all the data managed by the data service from the corresponding data providers
        /// </summary>
        Task LoadAsync();

        /// <summary>
        /// Saves all the data managed by the data service asynchronously
        /// (as soon as possible)
        /// </summary>
        Task SaveDataAsync();

        /// <summary>
        /// Saves all the data managed by the data service asynchronously,
        /// but only if no new save calls arrive within <c>_debounceTime</c>.
        /// </summary>
        void DebouncedSave();
    }
    public class ServiceDataService : BaseDataService, IServiceDataService
    {
        #region Private fields and the constructor

        private readonly IServiceDataProvider _serviceDataProvider;
        private readonly IServiceBookingDataProvider _serviceBookingDataProvider;

        public ServiceDataService(IServiceDataProvider serviceDataProvider,
            IServiceBookingDataProvider serviceBookingDataProvider, IMessageService messageService)
        {
            _serviceDataProvider = serviceDataProvider;
            _serviceBookingDataProvider = serviceBookingDataProvider;
            MessageService = messageService;
            ServiceBookingsForGuest = new ListCollectionView(ServiceBookings);
            ServiceBookingsForService = new ListCollectionView(ServiceBookings);
        }
        #endregion


        #region Public properties (see interface)

        public ObservableCollection<ServiceViewModel> Services { get; } = new();
        public ObservableCollection<ServiceBooking> ServiceBookings { get; } = new();
        public ListCollectionView ServiceBookingsForGuest { get; }
        public ListCollectionView ServiceBookingsForService { get; }
        public IMessageService MessageService { get; }

        #endregion


        #region Public method (see interface)

        public async Task LoadAsync()
        {
            var services = await _serviceDataProvider.LoadAsync();
            if (services is null)
                return;
            IdGenerator.Instance.CalculateMaxId(services);
            LoadCollection(Services, services, service => new ServiceViewModel(service, this));

            var serviceBookings = await _serviceBookingDataProvider.LoadAsync();
            LoadCollection(ServiceBookings, serviceBookings);
        }
        #endregion


        #region Protected method used by the base class for saving the data

        protected override async Task SaveCollectionsAsync()
        {
            await _serviceDataProvider.SaveAsync(Services.Select(x => x.GetService()));
            await _serviceBookingDataProvider.SaveAsync(ServiceBookings);
        }
        #endregion

    }
}
