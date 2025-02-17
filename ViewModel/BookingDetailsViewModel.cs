using System.Collections.ObjectModel;
using System.Windows;
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
        private readonly GuestViewModel _guest;
        private Booking? _booking;
        private Window _parentWindow;

        public BookingDetailsViewModel(Window parentWindow, string headerText, GuestViewModel guest, Booking? booking = null)
        {
            _parentWindow = parentWindow;
            _headerText = headerText;
            _guest = guest;
            _booking = booking;

            if (booking == null)
            {
                _checkInDate = DateTime.Today;
                _checkOutDate = DateTime.Today.AddDays(1);
                _roomId = -1;
            }
            else
            {
                _checkInDate = booking.CheckInDate;
                _checkOutDate = booking.CheckOutDate;
                _roomId = booking.RoomId;
            }
            
            ConfirmCommand = new DelegateCommand(Confirm, CanConfirm);
            InitializeRoomNames();
        }

        public string HeaderText
        {
            get => _headerText;
        }

        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (CheckInDate is not null)
                {
                    _checkInDate = (DateTime)value;
                    OnPropertyChanged();
                    ConfirmCommand.OnCanExecuteChanged();
                }
            }
        }

        public DateTime? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (CheckOutDate is not null)
                {
                    _checkOutDate = (DateTime)value;
                    OnPropertyChanged();
                    ConfirmCommand.OnCanExecuteChanged();
                }
            }
        }

        public string? RoomName
        {
            get
            {
                if (_roomId == -1)
                    return null;
                return RoomsViewModel.Rooms.First(x => x.Id == _roomId).Name;
            }
            set
            {
                if (value is not null)
                {
                    _roomId = int.Parse(value.Split()[0]);
                    OnPropertyChanged();
                    ConfirmCommand.OnCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<string> RoomNames { get; } = new();

        public DelegateCommand ConfirmCommand { get; }


        private void Confirm(object? parameter)
        {
            foreach (var booking in _guest.Bookings)
            {
                if (booking == _booking)
                    continue;
                if (booking.CheckInDate < _checkOutDate && booking.CheckOutDate > _checkInDate)
                {
                    MessageBox.Show("This guest has an overlapping booking!");
                    return;
                }
            }

            var room = RoomsViewModel.Rooms.First(x => x.Id == _roomId);
            foreach (var booking in room.Bookings)
            {
                if (booking == _booking)
                    continue;
                if (booking.CheckInDate < _checkOutDate && booking.CheckOutDate > _checkInDate)
                {
                    MessageBox.Show("This room has an overlapping booking!");
                    return;
                }
            }

            if (_booking is null)
            {
                _booking = new Booking();
                GuestsViewModel.Bookings.Add(_booking);
                _guest.Bookings.Add(_booking);
                room.Bookings.Add(_booking);
            }
            _booking.GuestId = _guest.Id;
            _booking.RoomId = _roomId;
            _booking.CheckInDate = _checkInDate;
            _booking.CheckOutDate = _checkOutDate;
            GuestsViewModel.SortBookings(_guest.Bookings);
            _parentWindow.Close();
        }

        private bool CanConfirm(object? parameter) => !(CheckOutDate < DateTime.Today || RoomName is null || CheckInDate > CheckOutDate);

        private void InitializeRoomNames()
        {
            foreach (var room in RoomsViewModel.Rooms)
            {
                RoomNames.Add(room.Name);
            }
        }
    }
}
