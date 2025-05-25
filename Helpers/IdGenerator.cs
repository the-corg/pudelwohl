namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers
{
    /// <summary>
    /// Generates unique concurrent IDs.
    /// Thread-safe
    /// </summary>
    public sealed class IdGenerator
    {
        // Lazily-loaded thread-safe singleton
        private static readonly Lazy<IdGenerator> lazy = new Lazy<IdGenerator>(() => new IdGenerator());
        private IdGenerator()
        {
        }
        /// <summary>
        /// The single instance of the singleton
        /// </summary>
        public static IdGenerator Instance => lazy.Value;


        // IDs for each class by its name
        private readonly Dictionary<string, int> IdForClass = [];


        private static readonly object _lockObject = new();

        /// <summary>
        /// Increments (thread-safe) and return the next ID for class <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Class that needs the next ID (must implement IHasId)</typeparam>
        /// <returns>The next ID for class <typeparamref name="T"/></returns>
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

        /// <summary>
        /// Calculates and stores the maximum ID in <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">Type of the items in <paramref name="collection"/> (must implement IHasId)</typeparam>
        /// <param name="collection">The collection of <typeparamref name="T"/> items for which the maximum ID has to be calculated</param>
        public void CalculateMaxId<T>(IEnumerable<T> collection)
            where T : IHasId
        {
            var className = typeof(T).Name;
            IdForClass[className] = collection.Max(item => item.Id);
        }

    }
}
