
using System.ComponentModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class ServiceBooking
    {
        public int GuestId { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }

    }
}
