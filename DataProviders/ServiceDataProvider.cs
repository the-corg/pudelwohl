using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    /// <summary>
    /// Loads and saves the data for services
    /// </summary>
    public interface IServiceDataProvider
    {
        /// <summary>
        /// Loads the data from storage
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="Service"/></returns>
        Task<IEnumerable<Service>?> LoadAsync();

        /// <summary>
        /// Saves the data to storage
        /// </summary>
        /// <param name="services">Collection of data to save</param>
        Task SaveAsync(IEnumerable<Service> services);
    }

    public class ServiceDataProvider : BaseDataProvider<Service>, IServiceDataProvider
    {
        /// <summary>
        /// Loads the initial data from a .csv file
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="Service"/> that was loaded from a .csv file</returns>
        protected override async Task<IEnumerable<Service>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<Service>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Services.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new Service
                           {
                               Id = int.Parse(split[0]),
                               Name = split[1],
                               Description = split[2],
                               DurationMinutes = int.Parse(split[3]),
                               StartTime = split[4],
                               EndTime = split[5]
                           };
                newList.AddRange(data);
            }
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}
