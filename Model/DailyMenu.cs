﻿namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    /// <summary>
    /// Model class for a daily menu for a particular date
    /// </summary>
    public class DailyMenu
    {
        public DateOnly Date { get; set; }
        public required int[] Menu { get; set; }
    }
}
