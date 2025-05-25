using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    /// <summary>
    /// Loads and saves the data for guest menus
    /// </summary>
    public interface IGuestMenuDataProvider
    {
        /// <summary>
        /// Loads the data from storage
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="GuestMenu"/></returns>
        Task<IEnumerable<GuestMenu>?> LoadAsync();

        /// <summary>
        /// Saves the data to storage
        /// </summary>
        /// <param name="guestMenus">Collection of data to save</param>
        Task SaveAsync(IEnumerable<GuestMenu> guestMenus);
    }

    public class GuestMenuDataProvider : BaseDataProvider<GuestMenu>, IGuestMenuDataProvider
    {
        /// <summary>
        /// Loads the initial data from a .csv file
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="GuestMenu"/> that was loaded from a .csv file</returns>
        protected override async Task<IEnumerable<GuestMenu>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<GuestMenu>();

            using (var reader = File.OpenText("DemoDataFiles/Dogs and Menu Options.csv"))
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
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}

