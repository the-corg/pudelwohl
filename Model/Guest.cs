using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    /// <summary>
    /// Model class for a guest
    /// </summary>
    public class Guest : IHasId
    {
        public int Id { get; init; } = IdGenerator.Instance.GetNextId<Guest>();
        public required string Name { get; set; }
        public string? Breed { get; set; }
        public Gender Gender { get; set; }
        public string? CoatColor { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? FavoriteToy { get; set; }
        public EarFloppiness EarFloppiness { get; set; }
        public string? SpecialRequests { get; set; }
        public bool IsArchived { get; set; }
    }

    /// <summary>
    /// Contains all possible values of a guest's gender
    /// </summary>
    public enum Gender
    {
        Unknown = 0,
        Male,
        Female,
        Other
    }

    /// <summary>
    /// Contains all possible values of a guest's ear floppiness
    /// </summary>
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
