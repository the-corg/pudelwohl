using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public interface IServiceBookingDataProvider
    {
        Task<IEnumerable<ServiceBooking>?> GetAllAsync();
    }

    public class ServiceBookingDataProvider : IServiceBookingDataProvider
    {
        public async Task<IEnumerable<ServiceBooking>?> GetAllAsync()
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
            return newList;
        }
    }
}
