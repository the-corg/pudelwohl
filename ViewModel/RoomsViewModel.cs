using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomsViewModel : ViewModelBase
    {
        private RoomViewModel? _selectedRoom;
        private DateTime? _occupancyDate;

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

    }
}
