using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    /// <summary>
    /// Loads and saves the data for service bookings
    /// </summary>
    public interface IServiceBookingDataProvider
    {
        /// <summary>
        /// Loads the data from storage
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="ServiceBooking"/></returns>
        Task<IEnumerable<ServiceBooking>?> LoadAsync();

        /// <summary>
        /// Saves the data to storage
        /// </summary>
        /// <param name="serviceBookings">Collection of data to save</param>
        Task SaveAsync(IEnumerable<ServiceBooking> serviceBookings);
    }

    public class ServiceBookingDataProvider : BaseDataProvider<ServiceBooking>, IServiceBookingDataProvider
    {
        /// <summary>
        /// Loads the initial data from a .csv file
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="ServiceBooking"/> that was loaded from a .csv file</returns>
        protected override async Task<IEnumerable<ServiceBooking>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<ServiceBooking>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Dogs and Services.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new ServiceBooking
                           {
                               GuestId = int.Parse(split[0]),
                               ServiceId = int.Parse(split[1]),
                               Date = DateOnly.Parse(split[2]),
                               StartTime = split[3]
                           };
                newList.AddRange(data);
            }
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}
