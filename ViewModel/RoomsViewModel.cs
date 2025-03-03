using System.Collections.ObjectModel;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomsViewModel : ViewModelBase
    {
        private RoomViewModel? _selectedRoom;
        IRoomDataService _roomDataService;

        public RoomsViewModel(IRoomDataService roomDataService)
        {
            _roomDataService = roomDataService;
            Rooms = roomDataService.Rooms;
            roomDataService.OccupancyDate = DateOnly.FromDateTime(DateTime.Now);
        }

        public ObservableCollection<RoomViewModel> Rooms { get; }

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
