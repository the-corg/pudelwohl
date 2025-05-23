﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for the Rooms tab
    /// </summary>
    public class RoomsViewModel : ViewModelBase
    {
        #region Private fields

        private RoomViewModel? _selectedRoom;
        private readonly IRoomDataService _roomDataService;
        private readonly IBookingDialogService _bookingDialogService;
        #endregion


        #region Constructor

        public RoomsViewModel(IRoomDataService roomDataService, IBookingDialogService bookingDialogService)
        {
            _roomDataService = roomDataService;
            _bookingDialogService = bookingDialogService;
            Rooms = roomDataService.Rooms;
            roomDataService.OccupancyDate = DateOnly.FromDateTime(DateTime.Now);

            AddBookingCommand = new DelegateCommand(execute => AddBooking());

            BookingsCollectionView = roomDataService.BookingsForRoom;
            // Filter bookings based on the selected room
            BookingsCollectionView.Filter =
                booking => (SelectedRoom is not null) && (((Booking)booking).RoomId == SelectedRoom.Id);
            // And sort it by check-in date, then check-out date
            BookingsCollectionView.SortDescriptions.Add(new SortDescription("CheckInDate", ListSortDirection.Ascending));
            BookingsCollectionView.SortDescriptions.Add(new SortDescription("CheckOutDate", ListSortDirection.Ascending));

            // Composite collection to show on the Rooms tab, with an Add button after the bookings
            BookingsCompositeCollection = new CompositeCollection
            {
                    new CollectionContainer { Collection = BookingsCollectionView },
                    new Booking { RoomId = -1 } // Fake item for the Add button
            };
        }
        #endregion


        #region Public properties

        /// <summary>
        /// Command for adding a booking
        /// </summary>
        public DelegateCommand AddBookingCommand { get; }

        /// <summary>
        /// Sorted and filtered collection of bookings
        /// </summary>
        public ListCollectionView BookingsCollectionView { get; }

        /// <summary>
        /// Composite collection that consists of:
        /// 1. Sorted and filtered collection of bookings
        /// 2. One special item that should be replaced with the Add button
        /// </summary>
        public CompositeCollection BookingsCompositeCollection { get; }

        /// <summary>
        /// Collection of all existing rooms
        /// </summary>
        public ObservableCollection<RoomViewModel> Rooms { get; }

        /// <summary>
        /// Currently selected room
        /// </summary>
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

        /// <summary>
        /// The date selected by the user, based on which the room occupancy is shown
        /// </summary>
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

        /// <summary>
        /// Shows whether a room is currently selected
        /// (used for hiding the room details when no room is selected)
        /// </summary>
        public bool IsRoomSelected => SelectedRoom is not null;

        #endregion


        #region Private method (for Add booking command)

        private void AddBooking()
        {
            if (SelectedRoom is null)
                return;

            _bookingDialogService.ShowBookingDialog("New Booking", true, false, -1, SelectedRoom.Id);
        }
        #endregion

    }
}
