namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers
{
    // Generates unique concurrent IDs. Thread-safe
    public sealed class IdGenerator
    {
        // Lazily-loaded thread-safe singleton
        private static readonly Lazy<IdGenerator> lazy = new Lazy<IdGenerator>(() => new IdGenerator());
        private IdGenerator()
        {
        }
        public static IdGenerator Instance => lazy.Value;


        // IDs for each class by its name
        private readonly Dictionary<string, int> IdForClass = [];


        private static readonly object _lockObject = new();
        // Increment (thread-safe) and return the next ID for class T
        public int GetNextId<T>()
            where T : IHasId
        {
            var className = typeof(T).Name;

            if (!IdForClass.ContainsKey(className))
                IdForClass[className] = 0;

            lock (_lockObject)
            {
                IdForClass[className]++;
            }
            return IdForClass[className];
        }

        // Calculate and store the maximum ID in the collection
        public void CalculateMaxId<T>(IEnumerable<T> collection)
            where T : IHasId
        {
            var className = typeof(T).Name;
            IdForClass[className] = collection.Max(item => item.Id);
        }

    }
}
