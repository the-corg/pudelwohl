using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataProviders
{
    /*public interface IDailyMenuDataProvider
    {
        Task<IEnumerable<DailyMenu>?> GetAllAsync();
    }

    public class DailyMenuDataProvider : IDailyMenuDataProvider
    {
        public async Task<IEnumerable<DailyMenu>?> GetAllAsync()
        {
            var newList = new List<DailyMenu>();

            using (var reader = File.OpenText("Data/DataFiles/DailyMenus.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new DailyMenu
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
            return newList;
        }
    }*/
}
