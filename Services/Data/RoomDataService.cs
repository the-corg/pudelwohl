﻿using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public interface IRoomDataService
    {
        ObservableCollection<RoomViewModel> Rooms { get; }
        ObservableCollection<Booking> Bookings { get; }
        ListCollectionView BookingsForGuest { get; }
        ListCollectionView BookingsForRoom { get; }
        DateOnly OccupancyDate { get; set; }
        string FreeRoomsToday { get; }
        Action? FreeRoomsUpdated { get; set; }
        void UpdateBookingData();
        Task LoadAsync();
        Task SaveDataAsync();
        void DebouncedSave();
    }
    public class RoomDataService : BaseDataService, IRoomDataService
    {
        private readonly IRoomDataProvider _roomDataProvider;
        private readonly IBookingDataProvider _bookingDataProvider;

        public RoomDataService(IRoomDataProvider roomDataProvider, IBookingDataProvider bookingDataProvider)
        {
            _roomDataProvider = roomDataProvider;
            _bookingDataProvider = bookingDataProvider;
            BookingsForGuest = new ListCollectionView(Bookings);
            BookingsForRoom = new ListCollectionView(Bookings);
        }

        public ObservableCollection<RoomViewModel> Rooms { get; } = new();
        public ObservableCollection<Booking> Bookings { get; } = new();
        public ListCollectionView BookingsForGuest { get; }
        public ListCollectionView BookingsForRoom { get; }

        // Used in RoomsViewModel for Binding with the DatePicker above the rooms ListView
        public DateOnly OccupancyDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

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

        // Called on Bookings.CollectionChanged
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

        protected override async Task SaveCollectionsAsync()
        {
            await _bookingDataProvider.SaveAsync(Bookings);
            // No need to save Rooms because they never change
        }

    }
}
