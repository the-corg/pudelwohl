﻿namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    /// <summary>
    /// Model class for a time slot
    /// </summary>
    public class TimeSlot
    {
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }

        public override string ToString()
        {
            return StartTime + " – " + EndTime;
        }
    }
}
