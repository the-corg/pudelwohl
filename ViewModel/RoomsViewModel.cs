
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomsViewModel : ViewModelBase
    {
        private readonly IRoomDataProvider _roomDataProvider;
        private RoomViewModel? _selectedRoom;
        private DateTime? _occupancyDate;

        private static readonly PropertyChangedEventArgs FreeRoomsTodayPropertyEventArgs = new PropertyChangedEventArgs(nameof(FreeRoomsToday));
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        public RoomsViewModel(IRoomDataProvider roomDataProvider)
        {
            _roomDataProvider = roomDataProvider;
            _occupancyDate = DateTime.Now;
        }

        public static ObservableCollection<RoomViewModel> Rooms { get; } = new();

        public RoomViewModel? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRoomSelected));
            }
        }

        public DateTime? OccupancyDate
        {
            get => _occupancyDate;
            set
            {
                _occupancyDate = value;
                OnPropertyChanged();
                CollectionViewSource.GetDefaultView(Rooms).Refresh();
            }
        }

        public bool IsRoomSelected => SelectedRoom is not null;

        public static int FreeRoomsToday
        {
            get
            {
                var today = DateTime.Now;
                int freeRooms = Rooms.Count;

                foreach (var room in Rooms)
                {
                    foreach(var booking in room.Bookings)
                    {
                        if (booking.CheckInDate <= today && booking.CheckOutDate >= today)
                        {
                            freeRooms--;
                            break;
                        }
                    }
                }
                return freeRooms;
            }
        }


        public static void BookingsChanged()
        {
            foreach (var room in Rooms)
            {
                room.BookingsChanged();
            }
            CollectionViewSource.GetDefaultView(Rooms).Refresh();
            StaticPropertyChanged?.Invoke(null, FreeRoomsTodayPropertyEventArgs);
        }

        public async Task LoadAsync()
        {
            if (Rooms.Count > 0)
                return;

            var rooms = await _roomDataProvider.GetAllAsync();
            if (rooms is not null)
            {
                foreach (var room in rooms)
                {
                    Rooms.Add(new RoomViewModel(room, this));
                }
            }

            var bookings = await BookingDataProvider.GetAllAsync();
            if (bookings is not null)
            {
                foreach (var booking in bookings)
                {
                    Rooms.First(x => x.Id == booking.RoomId).Bookings.Add(booking);
                }
            }
            RoomsViewModel.BookingsChanged();
        }
    }
}
