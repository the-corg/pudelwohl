using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public MainViewModel(IGuestDataProvider guestDataProvider, IRoomDataProvider roomDataProvider,
            IServiceDataProvider serviceDataProvider, IMealOptionDataProvider mealOptionDataProvider,
            IBookingDataProvider bookingDataProvider, IServiceBookingDataProvider serviceBookingDataProvider,
            IGuestMenuDataProvider guestMenuDataProvider)
        {
            _guestDataProvider = guestDataProvider;
            _roomDataProvider = roomDataProvider;
            _serviceDataProvider = serviceDataProvider;
            _mealOptionDataProvider = mealOptionDataProvider;
            _bookingDataProvider = bookingDataProvider;
            _serviceBookingDataProvider = serviceBookingDataProvider;
            _guestMenuDataProvider = guestMenuDataProvider;
            GuestsViewModel = new GuestsViewModel(Guests, Bookings, ServiceBookings, GuestMenus, this);
            RoomsViewModel = new RoomsViewModel(Rooms);
            ServicesViewModel = new ServicesViewModel(Services, this);
            MealOptionsViewModel = new MealOptionsViewModel(MealOptions, this);
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

        // Property to show the number of free rooms (and occupied rooms) in the main window's status bar
        public string FreeRoomsToday
        {
            get
            {
                var today = DateTime.Today;
                int freeRooms = Rooms.Count;
                string occupiedRooms = "";

                foreach (var room in Rooms)
                {
                    foreach (var booking in Bookings)
                    {
                        if (booking.RoomId == room.Id && booking.CheckInDate <= today && booking.CheckOutDate >= today)
                        {
                            freeRooms--;
                            occupiedRooms += room.Id + ", ";
                            break;
                        }
                    }
                }
                string result = "Free rooms today: " + freeRooms.ToString() + " out of " + Rooms.Count.ToString();
                if (freeRooms < Rooms.Count)
                {
                    // Part of the string that shows the IDs of the occupied rooms
                    occupiedRooms = occupiedRooms.Substring(0, occupiedRooms.Length - 2); // Delete the last ", "
                    result += ".  Occupied room";
                    if (Rooms.Count - freeRooms > 1)
                        result += "s"; // Make rooms plural. Take that, localization engineers!
                    result += ": " + occupiedRooms;
                }
                return result;
            }
        }

        public void BookingsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FreeRoomsToday));

            // Update room occupancy colors on the Rooms tab
            foreach (var room in Rooms)
                room.BookingsChanged();
        }

        public async Task LoadAsync()
        {
            LoadCollectionVMAsync<GuestViewModel, Guest>(Guests, await _guestDataProvider.GetAllAsync());
            LoadCollectionVMAsync<RoomViewModel, Room>(Rooms, await _roomDataProvider.GetAllAsync());
            LoadCollectionVMAsync<ServiceViewModel, Service>(Services, await _serviceDataProvider.GetAllAsync());
            LoadCollectionVMAsync<MealOptionViewModel, MealOption>(MealOptions, await _mealOptionDataProvider.GetAllAsync());
            LoadCollectionAsync<Booking>(Bookings, await _bookingDataProvider.GetAllAsync());
            LoadCollectionAsync<ServiceBooking>(ServiceBookings, await _serviceBookingDataProvider.GetAllAsync());
            //LoadCollectionAsync<GuestMenu>(GuestMenus, await _guestMenuDataProvider.GetAllAsync());
            // Add an event handler to refersh FreeRoomsToday in the status bar
            Bookings.CollectionChanged += BookingsChanged;
            // Everything loaded - refresh FreeRoomsToday once
            OnPropertyChanged(nameof(FreeRoomsToday));
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
        private void LoadCollectionVMAsync<VMT, T>(ObservableCollection<VMT> collection, IEnumerable<T>? data)
            where VMT : class
            where T : class
        {
            if (collection.Count > 0 || data is null)
                return;

            foreach (var element in data)
            {
                collection.Add((VMT)Activator.CreateInstance(typeof(VMT), element, this));
            }
        }

    }
}
