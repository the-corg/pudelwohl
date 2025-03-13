using System.IO;
using System.Text.Json;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public abstract class BaseDataProvider<T>
        where T : class
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        protected static readonly string _appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pudelwohl");
        protected static readonly string _filePath = Path.Combine(_appDataFolder, typeof(T).Name + "s.dat");

        protected async Task<IEnumerable<T>?> LoadFromJsonAsync()
        {
            string json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<IEnumerable<T>>(json) ?? [];
        }

        protected abstract Task<IEnumerable<T>?> LoadInitialDataFromCsvAsync();

        public async Task<IEnumerable<T>?> LoadAsync()
        {
            return File.Exists(_filePath) ? await LoadFromJsonAsync() : await LoadInitialDataFromCsvAsync();
        }

        public async Task SaveAsync(IEnumerable<T> collection)
        {
            Directory.CreateDirectory(_appDataFolder); // Ensure that the folder exists
            string json = JsonSerializer.Serialize(collection, jsonSerializerOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
