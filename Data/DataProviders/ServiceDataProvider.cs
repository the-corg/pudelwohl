using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataProviders
{
    public interface IServiceDataProvider
    {
        Task<IEnumerable<Service>?> GetAllAsync();
    }

    public class ServiceDataProvider : IServiceDataProvider
    {
        public async Task<IEnumerable<Service>?> GetAllAsync()
        {
            var newList = new List<Service>();

            using (var reader = File.OpenText("Data/DataFiles/Services.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new Service
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
            Service.CalculateNextId(newList);
            return newList;
        }
    }
}
