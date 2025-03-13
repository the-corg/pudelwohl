using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public interface IServiceBookingDataProvider
    {
        Task<IEnumerable<ServiceBooking>?> LoadAsync();
        Task SaveAsync(IEnumerable<ServiceBooking> serviceBookings);
    }

    public class ServiceBookingDataProvider : BaseDataProvider<ServiceBooking>, IServiceBookingDataProvider
    {
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
