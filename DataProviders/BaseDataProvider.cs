using System.IO;
using System.Text.Json;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    /// <summary>
    /// Base class for data providers.
    /// Provides all the methods for loading and saving the data,
    /// except for the method that loads the initial data from .csv files,
    /// which every inheriting data provider has to implement on its own.
    /// </summary>
    /// <typeparam name="T">Model class for the data</typeparam>
    public abstract class BaseDataProvider<T>
        where T : class
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        protected static readonly string _appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pudelwohl");
        protected static readonly string _filePath = Path.Combine(_appDataFolder, typeof(T).Name + "s.dat");

        /// <summary>
        /// Deserializes the data of type <typeparamref name="T"/> from JSON
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="T"/> that was deserialized from JSON</returns>
        protected async Task<IEnumerable<T>?> LoadFromJsonAsync()
        {
            string json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<IEnumerable<T>>(json) ?? [];
        }

        /// <summary>
        /// Loads the initial data from a .csv file.
        /// Every inheriting data provider has to provide an implementation for this method
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="T"/> that was loaded from a .csv file</returns>
        protected abstract Task<IEnumerable<T>?> LoadInitialDataFromCsvAsync();

        /// <summary>
        /// Loads the data, either from JSON or from CSV (if the JSON file doesn't exist)
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="T"/></returns>
        public async Task<IEnumerable<T>?> LoadAsync()
        {
            return File.Exists(_filePath) ? await LoadFromJsonAsync() : await LoadInitialDataFromCsvAsync();
        }

        /// <summary>
        /// Saves the data in <paramref name="collection"/> (serializing it to JSON)
        /// </summary>
        /// <param name="collection">The collection of items of type <typeparamref name="T"/> that has to be saved</param>
        public async Task SaveAsync(IEnumerable<T> collection)
        {
            Directory.CreateDirectory(_appDataFolder); // Ensure that the folder exists
            string json = JsonSerializer.Serialize(collection, jsonSerializerOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
