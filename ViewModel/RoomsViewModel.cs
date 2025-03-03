﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomsViewModel : ViewModelBase
    {
        private RoomViewModel? _selectedRoom;
        private readonly IRoomDataService _roomDataService;

        public RoomsViewModel(IRoomDataService roomDataService)
        {
            _roomDataService = roomDataService;
            Rooms = roomDataService.Rooms;
            Bookings = roomDataService.Bookings;
            roomDataService.OccupancyDate = DateOnly.FromDateTime(DateTime.Now);

            BookingsCollectionView = roomDataService.BookingsForRoom;
            // Filter bookings based on the selected room
            BookingsCollectionView.Filter =
                booking => (SelectedRoom is not null) && (((Booking)booking).RoomId == SelectedRoom.Id);
            // And sort it by check-in date, then check-out date
            BookingsCollectionView.SortDescriptions.Add(new SortDescription("CheckInDate", ListSortDirection.Ascending));
            BookingsCollectionView.SortDescriptions.Add(new SortDescription("CheckOutDate", ListSortDirection.Ascending));
        }

        public ListCollectionView BookingsCollectionView { get; }
        public ObservableCollection<RoomViewModel> Rooms { get; }
        public ObservableCollection<Booking> Bookings { get; }

        public RoomViewModel? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (_selectedRoom == value)
                    return;

                _selectedRoom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRoomSelected));
                BookingsCollectionView.Refresh();
            }
        }

        public DateOnly OccupancyDate
        {
            get => _roomDataService.OccupancyDate;
            set
            {
                if (_roomDataService.OccupancyDate == value)
                    return;

                _roomDataService.OccupancyDate = value;
                OnPropertyChanged();
                CollectionViewSource.GetDefaultView(Rooms).Refresh();
            }
        }

        // Used for hiding the room details when no room is selected
        public bool IsRoomSelected => SelectedRoom is not null;

    }
}
