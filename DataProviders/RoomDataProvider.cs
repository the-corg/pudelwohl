using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public interface IRoomDataProvider
    {
        Task<IEnumerable<Room>?> LoadAsync();
        Task SaveAsync(IEnumerable<Room> rooms);
    }

    public class RoomDataProvider : BaseDataProvider<Room>, IRoomDataProvider
    {
        protected override async Task<IEnumerable<Room>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<Room>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Rooms.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new Room
                           {
                               Id = int.Parse(split[0]),
                               Name = split[1],
                               Type = split[2],
                               Description = split[3],
                               MaxGuests = int.Parse(split[4])
                           };
                newList.AddRange(data);
            }
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}
