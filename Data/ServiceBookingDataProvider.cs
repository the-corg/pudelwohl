using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data
{
    public static class ServiceBookingDataProvider
    {
        private static bool allDone = false;
        public static List<ServiceBooking>? Data;
        public static async Task<IEnumerable<ServiceBooking>?> GetAllAsync()
        {
            if (Data is null)
            {
                Data = new();

                using (var reader = File.OpenText("Data/DataFiles/Dogs and Services.csv"))
                {
                    var fileText = await reader.ReadToEndAsync();
                    var lines = fileText.Split(Environment.NewLine);

                    var data = from line in lines.Skip(1)
                               let split = line.Split('|')
                               select new ServiceBooking
                               {
                                   GuestId = int.Parse(split[0]),
                                   ServiceId = int.Parse(split[1]),
                                   Date = DateTime.Parse(split[2]),
                                   StartTime = split[3]
                               };
                    Data.AddRange(data);
                }
                allDone = true;
            }
            while (!allDone)
            {
                await Task.Delay(10);
            }
            return Data;
        }
    }
}
