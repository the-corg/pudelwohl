using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomsViewModel : ViewModelBase
    {
        private RoomViewModel? _selectedRoom;
        private DateTime? _occupancyDate;

        // The property changed event for the static property FreeRoomsToday
        // to show the number of free rooms in the main window's status bar
        private static readonly PropertyChangedEventArgs FreeRoomsTodayPropertyEventArgs = new PropertyChangedEventArgs(nameof(FreeRoomsToday));
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        public RoomsViewModel(ObservableCollection<RoomViewModel> rooms)
        {
            Rooms = rooms;
            _occupancyDate = DateTime.Now;
        }

        public ObservableCollection<RoomViewModel> Rooms { get; }

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

        // Used for hiding the room details when no room is selected
        public bool IsRoomSelected => SelectedRoom is not null;

        // Static property to show the number of free rooms in the main window's status bar
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

    }
}
