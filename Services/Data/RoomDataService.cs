using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    /// <summary>
    /// Manages all data related to rooms
    /// </summary>
    public interface IRoomDataService
    {
        /// <summary>
        /// The collection of rooms
        /// </summary>
        ObservableCollection<RoomViewModel> Rooms { get; }

        /// <summary>
        /// The collection of bookings
        /// </summary>
        ObservableCollection<Booking> Bookings { get; }

        /// <summary>
        /// Sorted and filtered view for the collection of bookings, with bookings for only one guest
        /// </summary>
        ListCollectionView BookingsForGuest { get; }

        /// <summary>
        /// Sorted and filtered view for the collection of bookings, with bookings for only one room
        /// </summary>
        ListCollectionView BookingsForRoom { get; }

        /// <summary>
        /// Currently selected date for room occupancy data on the Rooms tab
        /// (used in RoomsViewModel for Binding with the DatePicker above the rooms ListView)
        /// </summary>
        DateOnly OccupancyDate { get; set; }

        /// <summary>
        /// Text represe
        /// // 
        /// </summary>

        /// <summary>
        /// String representation of the number of free rooms with a list of occupied rooms
        /// (used in MainViewModel for the status bar)
        /// </summary>
        string FreeRoomsToday { get; }

        /// <summary>
        /// The delegate to be invoked when the number of free rooms has changed
        /// </summary>
        Action? FreeRoomsUpdated { get; set; }

        /// <summary>
        /// To be called when booking data should be updated
        /// (called on Bookings.CollectionChanged)
        /// (updates the info in the status bar and the room occupancy data)
        /// </summary>
        void UpdateBookingData();

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
    public class RoomDataService : BaseDataService, IRoomDataService
    {
        #region Private fields and the constructor

        private readonly IRoomDataProvider _roomDataProvider;
        private readonly IBookingDataProvider _bookingDataProvider;

        public RoomDataService(IRoomDataProvider roomDataProvider, IBookingDataProvider bookingDataProvider)
        {
            _roomDataProvider = roomDataProvider;
            _bookingDataProvider = bookingDataProvider;
            BookingsForGuest = new ListCollectionView(Bookings);
            BookingsForRoom = new ListCollectionView(Bookings);
        }
        #endregion


        #region Public properties (see interface)

        public ObservableCollection<RoomViewModel> Rooms { get; } = new();
        public ObservableCollection<Booking> Bookings { get; } = new();
        public ListCollectionView BookingsForGuest { get; }
        public ListCollectionView BookingsForRoom { get; }

        public DateOnly OccupancyDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

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
                    occupiedRooms = occupiedRooms[..^2]; // Delete the last ", "
                    result += ".  Occupied room";
                    if (Rooms.Count - freeRooms > 1)
                        result += "s"; // Make rooms plural. Take that, localization engineers!
                    result += ": " + occupiedRooms;
                }
                return result;
            }
        }

        public Action? FreeRoomsUpdated { get; set; }

        #endregion


        #region Public methods (see interface)

        public void UpdateBookingData()
        {
            // Update FreeRoomsToday in the status bar
            FreeRoomsUpdated?.Invoke();
            // Update the room occupancy colors on the Rooms tab
            CollectionViewSource.GetDefaultView(Rooms).Refresh();
        }


        public async Task LoadAsync()
        {
            var rooms = await _roomDataProvider.LoadAsync();
            LoadCollection(Rooms, rooms, room => new RoomViewModel(room, this));

            var bookings = await _bookingDataProvider.LoadAsync();
            LoadCollection(Bookings, bookings);

            // Add this handler only after the bookings have been loaded
            // to prevent a ton of events while populating the collection
            Bookings.CollectionChanged += (s, e) => UpdateBookingData();
            UpdateBookingData();
        }
        #endregion


        #region Protected method used by the base class for saving the data

        protected override async Task SaveCollectionsAsync()
        {
            await _bookingDataProvider.SaveAsync(Bookings);
            // No need to save Rooms because they never change
        }
        #endregion

    }
}
