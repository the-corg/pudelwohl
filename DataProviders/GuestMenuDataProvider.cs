using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public interface IGuestMenuDataProvider
    {
        Task<IEnumerable<GuestMenu>?> GetAllAsync();
    }

    public class GuestMenuDataProvider : IGuestMenuDataProvider
    {
        public async Task<IEnumerable<GuestMenu>?> GetAllAsync()
        {
            var newList = new List<GuestMenu>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Dogs and Menu Options.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new GuestMenu
                           {
                               Date = DateOnly.Parse(split[0]),
                               GuestId = int.Parse(split[1]),
                               Breakfast = (split[2].Length > 0) ? int.Parse(split[2]) : 0,
                               Lunch = (split[3].Length > 0) ? int.Parse(split[3]) : 0,
                               Snack = (split[4].Length > 0) ? int.Parse(split[4]) : 0,
                               Dinner = (split[5].Length > 0) ? int.Parse(split[5]) : 0
                           };
                newList.AddRange(data);
            }
            return newList;
        }
    }
}

