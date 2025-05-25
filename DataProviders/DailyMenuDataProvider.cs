using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    /// <summary>
    /// Loads and saves the data for daily menus
    /// </summary>
    public interface IDailyMenuDataProvider
    {
        /// <summary>
        /// Loads the data from storage
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="DailyMenu"/></returns>
        Task<IEnumerable<DailyMenu>?> LoadAsync();

        /// <summary>
        /// Saves the data to storage
        /// </summary>
        /// <param name="dailyMenus">Collection of data to save</param>
        Task SaveAsync(IEnumerable<DailyMenu> dailyMenus);
    }

    public class DailyMenuDataProvider : BaseDataProvider<DailyMenu>, IDailyMenuDataProvider
    {
        /// <summary>
        /// Loads the initial data from a .csv file
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="DailyMenu"/> that was loaded from a .csv file</returns>
        protected override async Task<IEnumerable<DailyMenu>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<DailyMenu>();

            using (var reader = File.OpenText("DemoDataFiles/Daily Menus and Food IDs.csv"))
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
