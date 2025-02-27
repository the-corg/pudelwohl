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
        private DateTime _checkInDate;
        private DateTime _checkOutDate;
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
                _checkInDate = DateTime.Today;
                _checkOutDate = DateTime.Today.AddDays(1);
                _roomId = -1;
            }
            else
            {
                // Edit an old booking
                _checkInDate = booking.CheckInDate;
                _checkOutDate = booking.CheckOutDate;
                _roomId = booking.RoomId;
            }

            ConfirmCommand = new DelegateCommand(Confirm, CanConfirm);
            InitializeRoomNames();
        }
        public string HeaderText => _headerText;

        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (value is null || (DateTime)value == _checkInDate)
                    return;

                _checkInDate = (DateTime)value;
                OnPropertyChanged();
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public DateTime? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (value is null || (DateTime)value == _checkOutDate)
                    return;

                _checkOutDate = (DateTime)value;
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


        private void Confirm(object? parameter)
        {
            foreach (var booking in _mainViewModel.Bookings)
            {
                // Ignore this exact booking as well as bookings for other guests
                if (booking == _booking || booking.GuestId != _guestId)
                    continue;

                // Check if the date intervals overlap
                if (booking.CheckInDate < _checkOutDate && booking.CheckOutDate > _checkInDate)
                {
                    MessageBox.Show("This guest has an overlapping booking!");
                    return;
                }
            }

            foreach (var booking in _mainViewModel.Bookings)
            {
                // Ignore this exact booking as well as bookings for other rooms
                if (booking == _booking || booking.RoomId != _roomId)
                    continue;

                // Check if the date intervals overlap
                if (booking.CheckInDate < _checkOutDate && booking.CheckOutDate > _checkInDate)
                {
                    MessageBox.Show("This room has an overlapping booking!");
                    return;
                }
            }

            if (_booking is null)
            {
                _booking = new Booking();
                _mainViewModel.Bookings.Add(_booking);
            }
            _booking.GuestId = _guestId;
            _booking.RoomId = _roomId;
            _booking.CheckInDate = _checkInDate;
            _booking.CheckOutDate = _checkOutDate;

            // TODO: Check that this will refresh the bookings view on the Rooms tab (esp on Edit)
            // Have to call these manually to make the collection react to and Edit of a booking,
            // which otherwise wouldn't produce a collection change event
            CollectionViewSource.GetDefaultView(_mainViewModel.Bookings).Refresh();
            _mainViewModel.BookingsChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            _parentWindow.Close();
        }

        // Make Confirm button inactive if the check-out date is earlier than today,
        // no room name is entered, or the check-out-date is earlier than the check-in date
        private bool CanConfirm(object? parameter) =>
            !(CheckOutDate < DateTime.Today || RoomName is null || CheckInDate > CheckOutDate);

        private void InitializeRoomNames()
        {
            foreach (var room in _mainViewModel.Rooms)
            {
                RoomNames.Add(room.Name);
            }
        }
    }
}
