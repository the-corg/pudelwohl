using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class Service : IHasId
    {
        public int Id { get; init; } = IdGenerator.Instance.GetNextId<Service>();
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}
