using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data
{
    public static class BookingDataProvider
    {
        private static bool allDone = false;
        public static List<Booking>? Data;
        public static async Task<IEnumerable<Booking>?> GetAllAsync()
        {
            if (Data is null)
            {
                Data = new();

                using (var reader = File.OpenText("Data/DataFiles/Bookings.csv"))
                {
                    var fileText = await reader.ReadToEndAsync();
                    var lines = fileText.Split(Environment.NewLine);

                    var data = from line in lines.Skip(1)
                               let split = line.Split('|')
                               select new Booking
                               {
                                   GuestId = int.Parse(split[0]),
                                   RoomId = int.Parse(split[1]),
                                   CheckInDate = DateTime.Parse(split[2]),
                                   CheckOutDate = DateTime.Parse(split[3])
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
