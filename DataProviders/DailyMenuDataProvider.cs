using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public interface IDailyMenuDataProvider
    {
        Task<IEnumerable<DailyMenu>?> LoadAsync();
        Task SaveAsync(IEnumerable<DailyMenu> dailyMenus);
    }

    public class DailyMenuDataProvider : BaseDataProvider<DailyMenu>, IDailyMenuDataProvider
    {
        protected override async Task<IEnumerable<DailyMenu>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<DailyMenu>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Daily Menus and Food IDs.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new DailyMenu
                           {
                               Date = DateOnly.Parse(split[0]),
                               Menu = split.Skip(1).Select(x => int.Parse(x)).ToArray()
                           };
                newList.AddRange(data);
            }
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}
