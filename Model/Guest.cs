namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class Guest
    {
        public int Id { get; set; } = NextId;
        public string Name { get; set; }
        public string? Breed { get; set; }
        public Gender Gender { get; set; }
        public string? CoatColor { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? FavoriteToy { get; set; }
        public EarFloppiness EarFloppiness { get; set; }
        public string? SpecialRequests { get; set; }
        public bool IsArchived { get; set; }


        // Static field and property for automatic ID assignment
        private static int _nextId = 0;
        public static int NextId
        { 
            get => _nextId++;
            private set => _nextId = value;
        }

        private static bool nextIdAlreadyCalculated = false;
        // Calculate NextId based on the maximum of all IDs.
        // Works only once, should be called after initial data loading
        public static void CalculateNextId(List<Guest> guests)
        {
            if (nextIdAlreadyCalculated) 
                return;

            nextIdAlreadyCalculated = true;
            NextId = guests.Max(guest => guest.Id) + 1;
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
