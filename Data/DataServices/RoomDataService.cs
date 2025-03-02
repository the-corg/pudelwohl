using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataServices
{
    public interface IRoomDataService
    {
        ObservableCollection<RoomViewModel> Rooms { get; }
        ObservableCollection<Booking> Bookings { get; }
        DateOnly OccupancyDate { get; set; }
        string FreeRoomsToday { get; }
        Task LoadAsync();
    }
    public class RoomDataService : BaseDataService, IRoomDataService
    {
        private readonly IRoomDataProvider _roomDataProvider;
        private readonly IBookingDataProvider _bookingDataProvider;

        public RoomDataService(IRoomDataProvider roomDataProvider, IBookingDataProvider bookingDataProvider)
        {
            _roomDataProvider = roomDataProvider;
            _bookingDataProvider = bookingDataProvider;
        }

        public ObservableCollection<RoomViewModel> Rooms { get; } = new();
        public ObservableCollection<Booking> Bookings { get; } = new();

        // Used in RoomsViewModel for Binding with the DatePicker above the rooms ListView
        public DateOnly OccupancyDate { get; set ; } = DateOnly.FromDateTime(DateTime.Now);

        // Used in MainViewModel to show the number of free rooms (and the occupied rooms) in the status bar
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
            //TODO
            //OnPropertyChanged(nameof(FreeRoomsToday));

            // Update room occupancy colors on the Rooms tab
            foreach (var room in Rooms)
                room.BookingsChanged();
        }

        public async Task LoadAsync()
        {
            var rooms = await _roomDataProvider.GetAllAsync();
            LoadCollection(Rooms, rooms, room => new RoomViewModel(room, this));

            var bookings = await _bookingDataProvider.GetAllAsync();
            LoadCollection(Bookings, bookings);

            // Add an event handler to refersh FreeRoomsToday in the status bar
            Bookings.CollectionChanged += BookingsChanged;
            // Everything loaded - refresh FreeRoomsToday once
            //OnPropertyChanged(nameof(FreeRoomsToday));
        }
    }
}
