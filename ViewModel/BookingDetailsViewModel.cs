using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class BookingDetailsViewModel : ViewModelBase
    {
        private string _headerText;
        private DateOnly? _checkInDate;
        private DateOnly? _checkOutDate;
        private int _roomId;
        private readonly int _guestId;
        private Booking? _booking;
        private Window _parentWindow;
        private MainViewModel _mainViewModel;

        public BookingDetailsViewModel(MainViewModel mainViewModel, Window parentWindow,
            string headerText, int guestId, Booking? booking = null)
        {
            _mainViewModel = mainViewModel;
            _parentWindow = parentWindow;
            _headerText = headerText;
            _guestId = guestId;
            _booking = booking;

            if (booking == null)
            {
                // This will be a new booking
                _checkInDate = DateOnly.FromDateTime(DateTime.Now);
                _checkOutDate = _checkInDate?.AddDays(1);
                _roomId = -1;
            }
            else
            {
                // Edit an old booking
                _checkInDate = booking.CheckInDate;
                _checkOutDate = booking.CheckOutDate;
                _roomId = booking.RoomId;
            }

            ConfirmCommand = new DelegateCommand(execute => Confirm(), canExecute => CanConfirm());
            InitializeRoomNames();
        }
        public string HeaderText => _headerText;

        public DateOnly? CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (value == _checkInDate)
                    return;

                _checkInDate = value;
                OnPropertyChanged();
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public DateOnly? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (value == _checkOutDate)
                    return;

                _checkOutDate = value;
                OnPropertyChanged();
                ConfirmCommand.OnCanExecuteChanged();
            }
        }
        public string? RoomName
        {
            get
            {
                if (_roomId == -1)
                    return null;
                // This would crash intentionally if no room is found.
                // Rooms can't be deleted, so it would mean something is seriously wrong
                return _mainViewModel.Rooms.First(x => x.Id == _roomId).Name;
            }
            set
            {
                if (value is null)
                    return;

                // Get roomId by using the fact that room names start with RoomId
                var newRoomId = int.Parse(value.Split()[0]);
                if (newRoomId == _roomId)
                    return;

                _roomId = newRoomId;
                OnPropertyChanged();
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public ObservableCollection<string> RoomNames { get; } = new();

        public DelegateCommand ConfirmCommand { get; }


        // Make the Confirm button inactive if one of the dates or the room are missing,
        // or the check-out date is earlier than today, or the check-out date is earlier than the check-in date
        private bool CanConfirm() => !(CheckOutDate is null || CheckInDate is null ||
            CheckOutDate < DateOnly.FromDateTime(DateTime.Now) || RoomName is null || CheckInDate > CheckOutDate);
        private void Confirm()
        {
            if (_booking is not null)
            {
                // This is an Edit. Check if nothing was edited
                if (_booking.RoomId == _roomId && _booking.CheckInDate == _checkInDate && _booking.CheckOutDate == _checkOutDate)
                {
                    _parentWindow.Close();
                    return;
                }
            }

            // Check if the guest has a booking for this time period already
            foreach (var booking in _mainViewModel.Bookings)
            {
                // Ignore this exact booking as well as bookings for other guests
                if (booking == _booking || booking.GuestId != _guestId)
                    continue;

                // Check if the date intervals overlap
                if (booking.CheckInDate <= _checkOutDate && booking.CheckOutDate >= _checkInDate)
                {
                    MessageBox.Show("This guest has an overlapping booking!");
                    return;
                }
            }

            // Check if the room has a booking for this time period already

            // This would crash intentionally if no room is found.
            // Rooms can't be deleted, so it would mean something is seriously wrong
            var room = _mainViewModel.Rooms.First(x => x.Id == _roomId);
            // The dates can't be null, otherwise CanConfirm would've disabled the button
            var maxOccupants = room.MaxOccupantsWithinDates((DateOnly)_checkInDate, (DateOnly)_checkOutDate, _booking);
            if (maxOccupants >= room.MaxGuests)
            {
                MessageBox.Show("This room is full at least on some of the dates.\nThe booking cannot be created.", "Max capacity reached", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (maxOccupants > 0)
            {
                // The room is neither free nor full, ask the user
                var result = MessageBox.Show("Someone else has an overlapping booking for this room.\nAdd the booking anyway?", "Share the room?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;
            }

            // All checks done. Add or edit the booking now
            if (_booking is null)
            {
                _booking = new Booking();
                // This add creates a collection change event too early but a manual refersh below
                // is inevitable either way because an Edit doesn't create such an event at all
                _mainViewModel.Bookings.Add(_booking);
            }
            _booking.GuestId = _guestId;
            _booking.RoomId = _roomId;
            _booking.CheckInDate = (DateOnly)_checkInDate;
            _booking.CheckOutDate = (DateOnly)_checkOutDate;

            // Have to call these manually to make the collection react to an Edit of a booking,
            // which otherwise wouldn't produce a collection change event
            CollectionViewSource.GetDefaultView(_mainViewModel.Bookings).Refresh();
            _mainViewModel.BookingsChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            // Close the dialog
            _parentWindow.Close();
        }

        private void InitializeRoomNames()
        {
            foreach (var room in _mainViewModel.Rooms)
            {
                RoomNames.Add(room.Name);
            }
        }
    }
}
