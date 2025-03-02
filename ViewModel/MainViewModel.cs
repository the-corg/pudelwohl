using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataProviders;
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
            IGuestMenuDataProvider guestMenuDataProvider)//, GuestsViewModel guestsViewModel, RoomsViewModel roomsViewModel,
            //ServicesViewModel servicesViewModel, MealOptionsViewModel mealOptionsViewModel)
        {
            _guestDataProvider = guestDataProvider;
            _roomDataProvider = roomDataProvider;
            _serviceDataProvider = serviceDataProvider;
            _mealOptionDataProvider = mealOptionDataProvider;
            _bookingDataProvider = bookingDataProvider;
            _serviceBookingDataProvider = serviceBookingDataProvider;
            _guestMenuDataProvider = guestMenuDataProvider;
            GuestsViewModel = new GuestsViewModel(this);
            RoomsViewModel = new RoomsViewModel(this);
            ServicesViewModel = new ServicesViewModel(this);
            MealOptionsViewModel = new MealOptionsViewModel(this);
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
                var today = DateOnly.FromDateTime(DateTime.Now);
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
            LoadCollectionAsync(Guests, await _guestDataProvider.GetAllAsync(), guest => new GuestViewModel(guest, this));
            LoadCollectionAsync(Rooms, await _roomDataProvider.GetAllAsync(), room => new RoomViewModel(room, this));
            LoadCollectionAsync(Services, await _serviceDataProvider.GetAllAsync(), service => new ServiceViewModel(service, this));
            LoadCollectionAsync(MealOptions, await _mealOptionDataProvider.GetAllAsync(), mealOption => new MealOptionViewModel(mealOption, this));
            LoadCollectionAsync(Bookings, await _bookingDataProvider.GetAllAsync());
            LoadCollectionAsync(ServiceBookings, await _serviceBookingDataProvider.GetAllAsync());
            //LoadCollectionAsync<GuestMenu>(GuestMenus, await _guestMenuDataProvider.GetAllAsync());

            // Add an event handler to refersh FreeRoomsToday in the status bar
            Bookings.CollectionChanged += BookingsChanged;
            // Everything loaded - refresh FreeRoomsToday once
            OnPropertyChanged(nameof(FreeRoomsToday));
        }

        // Load elements from data into the corresponding collection using the wrapping function, when needed
        private void LoadCollectionAsync<TModel, TViewModel>(ObservableCollection<TViewModel> collection, 
            IEnumerable<TModel>? data, Func<TModel, TViewModel>? wrap = null)
            where TModel : class
            where TViewModel : class
        {
            if (collection.Count > 0 || data is null)
                return;

            foreach (var item in data)
            {
                collection.Add(wrap == null ? (item as TViewModel)! : wrap(item));
            }
        }

    }
}
