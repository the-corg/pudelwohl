﻿namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class GuestMenu
    {
        public int GuestId { get; set; }
        public DateOnly Date { get; set; }
        public int Breakfast { get; set; }
        public int Lunch { get; set; }
        public int Snack { get; set; }
        public int Dinner { get; set; }
    }
}
