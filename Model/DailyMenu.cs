namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class DailyMenu
    {
        public DateOnly Date { get; set; }
        public required int[] Breakfast { get; set; }
        public required int[] Lunch { get; set; }
        public required int[] Snack { get; set; }
        public required int[] Dinner { get; set; }
    }
}
