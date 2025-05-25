using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    /// <summary>
    /// Loads and saves the data for bookings
    /// </summary>
    public interface IBookingDataProvider
    {
        /// <summary>
        /// Loads the data from storage
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="Booking"/></returns>
        Task<IEnumerable<Booking>?> LoadAsync();

        /// <summary>
        /// Saves the data to storage
        /// </summary>
        /// <param name="bookings">Collection of data to save</param>
        Task SaveAsync(IEnumerable<Booking> bookings);
    }

    public class BookingDataProvider : BaseDataProvider<Booking>, IBookingDataProvider
    {
        /// <summary>
        /// Loads the initial data from a .csv file
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="Booking"/> that was loaded from a .csv file</returns>
        protected override async Task<IEnumerable<Booking>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<Booking>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Bookings.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new Booking
                           {
                               GuestId = int.Parse(split[0]),
                               RoomId = int.Parse(split[1]),
                               CheckInDate = DateOnly.Parse(split[2]),
                               CheckOutDate = DateOnly.Parse(split[3])
                           };
                newList.AddRange(data);
            }
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}
