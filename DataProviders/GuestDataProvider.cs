using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders
{
    public interface IGuestDataProvider
    {
        Task<IEnumerable<Guest>?> LoadAsync();
        Task SaveAsync(IEnumerable<Guest> guests);
    }

    public class GuestDataProvider : BaseDataProvider<Guest>, IGuestDataProvider
    {
        protected override async Task<IEnumerable<Guest>?> LoadInitialDataFromCsvAsync()
        {
            var newList = new List<Guest>();

            using (var reader = File.OpenText("DataProviders/DemoDataFiles/Dogs.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new Guest
                           {
                               Id = int.Parse(split[0]),
                               Name = split[1],
                               Breed = split[2],
                               Gender = Enum.Parse<Gender>(split[3]),
                               CoatColor = split[4],
                               DateOfBirth = DateOnly.Parse(split[5]),
                               FavoriteToy = split[6],
                               EarFloppiness = Enum.Parse<EarFloppiness>(split[7]),
                               SpecialRequests = split[8],
                               IsArchived = bool.Parse(split[9])
                           };
                newList.AddRange(data);
            }
            // Save everything to AppData\Local\Pudelwohl
            await SaveAsync(newList);
            return newList;
        }
    }
}
