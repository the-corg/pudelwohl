using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for a single room
    /// </summary>
    public class RoomViewModel : ViewModelBase
    {
        #region Private fields and the constructor

        private readonly Room _model;
        private readonly IRoomDataService _roomDataService;

        public RoomViewModel(Room model, IRoomDataService roomDataService)
        {
            _model = model;
            _roomDataService = roomDataService;
        }
        #endregion


        #region Public properties

        /// <summary>
        /// Id of the room
        /// </summary>
        public int Id => _model.Id;

        /// <summary>
        /// Name of the room
        /// </summary>
        public string? Name => _model.Name;

        /// <summary>
        /// Type of the room
        /// </summary>
        public string Type => _model.Type;

        /// <summary>
        /// Description of the room
        /// </summary>
        public string? Description => _model.Description;

        /// <summary>
        /// The maximum number of guests the room can have similtaneously
        /// </summary>
        public int MaxGuests => _model.MaxGuests;

        /// <summary>
        /// Shows whether the room is full on the date selected by the user
        /// </summary>
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

        /// <summary>
        /// Shows whether the room is free on the date selected by the user
        /// </summary>
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
        #endregion


        #region Public method
        /// <summary>
        /// Calculates the maximum number of occupants the room has
        /// between <paramref name="checkInDate"/> and <paramref name="checkOutDate"/>, with an option to ignore one particular booking
        /// </summary>
        /// <param name="checkInDate">The start date of the calculation period</param>
        /// <param name="checkOutDate">The end date of the calculation period</param>
        /// <param name="bookingToIgnore">If provided, this booking will be ignored</param>
        /// <returns>The maximum number of occupants the room has between the provided dates</returns>
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
        #endregion

    }
}
