
using System.ComponentModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Breed { get; set; }
        public Gender Gender { get; set; }
        public string? CoatColor { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? FavoriteToy { get; set; }
        public EarFloppiness EarFloppiness { get; set; }
        public string? SpecialRequests { get; set; }
        public List<Booking>? Bookings { get; set; }
        public List<Service>? Services { get; set; }
        public List<GuestMenu>? GuestMenus { get; set; }
        public bool IsArchived { get; set; }

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
