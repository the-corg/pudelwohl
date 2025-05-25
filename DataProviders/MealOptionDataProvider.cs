using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    /// <summary>
    /// Loads and saves the data for meal options
    /// </summary>
    public interface IMealOptionDataProvider
    {
        /// <summary>
        /// Loads the data from storage
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="MealOption"/></returns>
        Task<IEnumerable<MealOption>?> LoadAsync();

        /// <summary>
        /// Saves the data to storage
        /// </summary>
        /// <param name="mealOptions">Collection of data to save</param>
        Task SaveAsync(IEnumerable<MealOption> mealOptions);
    }

    public class MealOptionDataProvider : BaseDataProvider<MealOption>, IMealOptionDataProvider
    {
        /// <summary>
        /// Loads the initial data from a .csv file
        /// </summary>
        /// <returns>A collection of items of type <typeparamref name="MealOption"/> that was loaded from a .csv file</returns>
        protected override async Task<IEnumerable<MealOption>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<MealOption>();

            using (var reader = File.OpenText("DemoDataFiles/Meal Options.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new MealOption
                           {
                               Id = int.Parse(split[0]),
                               Name = split[1],
                               IsBreakfast = bool.Parse(split[2]),
                               IsLunch = bool.Parse(split[3]),
                               IsSnack = bool.Parse(split[4]),
                               IsDinner = bool.Parse(split[5])
                           };
                newList.AddRange(data);
            }
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}
