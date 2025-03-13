using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public interface IMealOptionDataProvider
    {
        Task<IEnumerable<MealOption>?> LoadAsync();
        Task SaveAsync(IEnumerable<MealOption> mealOptions);
    }

    public class MealOptionDataProvider : BaseDataProvider<MealOption>, IMealOptionDataProvider
    {
        protected override async Task<IEnumerable<MealOption>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<MealOption>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Meal Options.csv"))
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
