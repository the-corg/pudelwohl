using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IGuestDataProvider _guestDataProvider;
        private readonly IRoomDataProvider _roomDataProvider;
        private readonly IServiceDataProvider _serviceDataProvider;
        private readonly IMealOptionDataProvider _mealOptionDataProvider;
        private readonly IBookingDataProvider _bookingDataProvider;
        private readonly IServiceBookingDataProvider _serviceBookingDataProvider;
        private readonly IGuestMenuDataProvider _guestMenuDataProvider;

        public MainViewModel(GuestsViewModel guestsViewModel, RoomsViewModel roomsViewModel,
            ServicesViewModel servicesViewModel, MealOptionsViewModel mealOptionsViewModel,
            IGuestDataProvider guestDataProvider, IRoomDataProvider roomDataProvider,
            IServiceDataProvider serviceDataProvider, IMealOptionDataProvider mealOptionDataProvider,
            IBookingDataProvider bookingDataProvider, IServiceBookingDataProvider serviceBookingDataProvider,
            IGuestMenuDataProvider guestMenuDataProvider)
        {
            GuestsViewModel = guestsViewModel;
            RoomsViewModel = roomsViewModel;
            ServicesViewModel = servicesViewModel;
            MealOptionsViewModel = mealOptionsViewModel;
            _guestDataProvider = guestDataProvider;
            _roomDataProvider = roomDataProvider;
            _serviceDataProvider = serviceDataProvider;
            _mealOptionDataProvider = mealOptionDataProvider;
            _bookingDataProvider = bookingDataProvider;
            _serviceBookingDataProvider = serviceBookingDataProvider;
            _guestMenuDataProvider = guestMenuDataProvider;
        }

        public GuestsViewModel GuestsViewModel { get; }
        public RoomsViewModel RoomsViewModel { get; }
        public ServicesViewModel ServicesViewModel { get; }
        public MealOptionsViewModel MealOptionsViewModel { get; }


        public ObservableCollection<GuestViewModel> Guests { get; } = new();
        public ObservableCollection<RoomViewModel> Rooms { get; } = new();
        public ObservableCollection<ServiceViewModel> Services { get; } = new();
        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();
        public ObservableCollection<Booking> Bookings { get; } = new();
        public ObservableCollection<ServiceBooking> ServiceBookings { get; } = new();
        public ObservableCollection<GuestMenu> GuestMenus { get; } = new();


        public async Task LoadAsync()
        {
            LoadCollectionVMAsync<GuestViewModel, Guest>(Guests, await _guestDataProvider.GetAllAsync());
            LoadCollectionVMAsync<RoomViewModel, Room>(Rooms, await _roomDataProvider.GetAllAsync());
            LoadCollectionVMAsync<ServiceViewModel, Service>(Services, await _serviceDataProvider.GetAllAsync());
            LoadCollectionVMAsync<MealOptionViewModel, MealOption>(MealOptions, await _mealOptionDataProvider.GetAllAsync());
            LoadCollectionAsync<Booking>(Bookings, await _bookingDataProvider.GetAllAsync());
            LoadCollectionAsync<ServiceBooking>(ServiceBookings, await _serviceBookingDataProvider.GetAllAsync());
            LoadCollectionAsync<GuestMenu>(GuestMenus, await _guestMenuDataProvider.GetAllAsync());
        }

        // Loads elements provided in data to the corresponding ObservableCollection
        private static void LoadCollectionAsync<T>(ObservableCollection<T> collection, IEnumerable<T>? data)
            where T : class
        {
            if (collection.Count > 0 || data is null)
                return;

            foreach (var element in data)
            {
                collection.Add(element);
            }
        }

        // Loads item view models created from elements provided in data to the corresponding ObservableCollection
        private static void LoadCollectionVMAsync<VMT, T>(ObservableCollection<VMT> collection, IEnumerable<T>? data)
            where VMT : class
            where T : class
        {
            if (collection.Count > 0 || data is null)
                return;

            foreach (var element in data)
            {
                collection.Add((VMT)Activator.CreateInstance(typeof(VMT), element));
            }
        }

    }
}
