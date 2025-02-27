using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomViewModel : ViewModelBase
    {
        private readonly Room _model;
        private readonly MainViewModel _mainViewModel;

        public RoomViewModel(Room model, MainViewModel mainViewModel)
        {
            _model = model;
            _mainViewModel = mainViewModel;
        }

        public int Id => _model.Id;

        public string? Name => _model.Name;

        public string Type => _model.Type;

        public string? Description => _model.Description;

        public int MaxGuests => _model.MaxGuests;

        public bool IsFull
        {
            get
            {
                if (_mainViewModel.Bookings.Count == 0)
                    return false;
                
                int occupants = 0;
                // Count occupants in this room on the selected date
                foreach (Booking booking in _mainViewModel.Bookings)
                {
                    if (booking.RoomId == Id &&
                        booking.CheckInDate <= _mainViewModel.RoomsViewModel.OccupancyDate && 
                        booking.CheckOutDate >= _mainViewModel.RoomsViewModel.OccupancyDate)
                        occupants++;
                }

                return (occupants >= MaxGuests);
            }
        }

        public bool IsFree
        {
            get
            {
                if (_mainViewModel.Bookings.Count == 0)
                    return true;

                // Look for at least one occupant in this room on the selected date
                foreach (Booking booking in _mainViewModel.Bookings)
                {
                    if (booking.RoomId == Id && 
                        booking.CheckInDate <= _mainViewModel.RoomsViewModel.OccupancyDate && 
                        booking.CheckOutDate >= _mainViewModel.RoomsViewModel.OccupancyDate)
                        return false;
                }
                // No occupants found
                return true;
            }
        }

        // TODO!
        /*
        public string? BookingsString
        {
            get
            {
                if (Bookings.Count == 0)
                {
                    return "None";
                }

                var sortedBookings = Bookings.OrderBy(x => x.CheckInDate).ToList();

                string result = "";
                foreach (var booking in sortedBookings)
                {
                    if (booking.CheckOutDate < DateTime.Now)
                    {
                        result += "(PAST BOOKING) ";
                    }
                    var guest = GuestsViewModel.Guests.First(x => x.Id == booking.GuestId);
                    result += $"{booking.CheckInDate.ToLongDateString()} - {booking.CheckOutDate.ToLongDateString()}:\n        {guest.Name} ({guest.Breed})\n\n";
                }

                result = result.Substring(0, result.Length - 2); // delete the last two line breaks

                return result;
            }
        }*/

        // Called from the MainViewModel when bookings for this room change
        public void BookingsChanged()
        {
            OnPropertyChanged(nameof(IsFull));
            OnPropertyChanged(nameof(IsFree));
            // TODO! Probably better to have a Booking View in RoomsViewModel and refresh it there
            // from MainViewModel BookingsChange event handler once, instead of here for each room
            //OnPropertyChanged(nameof(BookingsString));
        }
    }
}
