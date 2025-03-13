namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class Service
    {
        public int Id { get; set; } = NextId;
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        // Private static field and property for automatic ID assignment
        private static int _nextId = 0;
        private static int NextId => _nextId++;

        private static bool nextIdAlreadyCalculated = false;
        // Calculate the next ID based on the maximum of all IDs.
        // Works only once, should be called after initial data loading
        public static void CalculateNextId(IEnumerable<Service> services)
        {
            if (nextIdAlreadyCalculated)
                return;

            nextIdAlreadyCalculated = true;
            _nextId = services.Max(service => service.Id) + 1;
        }
    }
}
