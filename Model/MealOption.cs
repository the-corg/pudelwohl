namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class MealOption
    {
        public int Id { get; set; } = NextId;
        public string Name { get; set; }
        public bool IsBreakfast { get; set; }
        public bool IsLunch { get; set; }
        public bool IsSnack { get; set; }
        public bool IsDinner { get; set; }


        // Private static field and property for automatic ID assignment
        private static int _nextId = 0;
        private static int NextId => _nextId++;

        private static bool nextIdAlreadyCalculated = false;
        // Calculate the next ID based on the maximum of all IDs.
        // Works only once, should be called after initial data loading
        public static void CalculateNextId(List<MealOption> mealOptions)
        {
            if (nextIdAlreadyCalculated)
                return;

            nextIdAlreadyCalculated = true;
            _nextId = mealOptions.Max(mealOption => mealOption.Id) + 1;
        }
    }
}
