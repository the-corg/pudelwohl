namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class Room
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Type { get; init; }
        public required string Description { get; init; }
        public required int MaxGuests { get; init; }
    }
}
