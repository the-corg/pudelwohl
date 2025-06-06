﻿namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    /// <summary>
    /// Model class for a service booking
    /// </summary>
    public class ServiceBooking
    {
        public int GuestId { get; set; }
        public int ServiceId { get; set; }
        public DateOnly Date { get; set; }
        public string? StartTime { get; set; }
    }
}
