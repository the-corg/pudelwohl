namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    /// <summary>
    /// Model class for a booking
    /// </summary>
    public class Booking
    {
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
    }
}
