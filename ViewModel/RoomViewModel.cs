using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataServices;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomViewModel : ViewModelBase
    {
        private readonly Room _model;
        private readonly IRoomDataService _roomDataService;

        public RoomViewModel(Room model, IRoomDataService roomDataService)
        {
            _model = model;
            _roomDataService = roomDataService;
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
                if (_roomDataService.Bookings.Count == 0)
                    return false;
                
                int occupants = 0;
                // Count occupants in this room on the selected date
                foreach (Booking booking in _roomDataService.Bookings)
                {
                    if (booking.RoomId == Id &&
                        booking.CheckInDate <= _roomDataService.OccupancyDate && 
                        booking.CheckOutDate >= _roomDataService.OccupancyDate)
                        occupants++;
                }

                return (occupants >= MaxGuests);
            }
        }

        public bool IsFree
        {
            get
            {
                if (_roomDataService.Bookings.Count == 0)
                    return true;

                // Look for at least one occupant in this room on the selected date
                foreach (Booking booking in _roomDataService.Bookings)
                {
                    if (booking.RoomId == Id && 
                        booking.CheckInDate <= _roomDataService.OccupancyDate && 
                        booking.CheckOutDate >= _roomDataService.OccupancyDate)
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
                    if (booking.CheckOutDate < DateTime.Today)
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

        public int MaxOccupantsWithinDates(DateOnly checkInDate, DateOnly checkOutDate, Booking? bookingToIgnore = null)
        {
            int maxOccupants = 0;

            // Loop through each date of the time period
            for (var day = checkInDate; day <= checkOutDate; day = day.AddDays(1))
            {
                int dayOccupants = 0;
                foreach (var booking in _roomDataService.Bookings)
                {
                    // Ignore bookingToIgnore as well as bookings for other rooms
                    if (booking == bookingToIgnore || booking.RoomId != Id)
                        continue;

                    // Check if the current day falls within the booking dates
                    if (booking.CheckInDate <= day && booking.CheckOutDate >= day)
                        dayOccupants++; // Found a booking for this room on this day
                }
                if (dayOccupants > maxOccupants)
                    maxOccupants = dayOccupants;
            }
            return maxOccupants;
        }

    }
}
