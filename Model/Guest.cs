namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class Guest
    {
        public int Id { get; set; } = NextId;
        public required string Name { get; set; }
        public string? Breed { get; set; }
        public Gender Gender { get; set; }
        public string? CoatColor { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? FavoriteToy { get; set; }
        public EarFloppiness EarFloppiness { get; set; }
        public string? SpecialRequests { get; set; }
        public bool IsArchived { get; set; }

        // Private static field and property for automatic ID assignment
        private static int _nextId = 0;
        private static int NextId => _nextId++;

        private static bool nextIdAlreadyCalculated = false;

        // Calculate the next ID based on the maximum of all IDs.
        // Works only once, should be called after initial data loading
        public static void CalculateNextId(List<Guest> guests)
        {
            if (nextIdAlreadyCalculated) 
                return;

            nextIdAlreadyCalculated = true;
            _nextId = guests.Max(guest => guest.Id) + 1;
        }
    }

    public enum Gender
    {
        Unknown = 0,
        Male,
        Female,
        Other
    }

    public enum EarFloppiness
    {
        Unknown = 0,
        Slightly_Floppy,
        Moderately_Floppy,
        Floppy,
        Very_Floppy,
        Extremely_Floppy
    }

}
