﻿using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data
{
    public interface IGuestDataProvider
    {
        Task<IEnumerable<Guest>?> GetAllAsync();
    }

    public class GuestDataProvider : IGuestDataProvider
    {
        public async Task<IEnumerable<Guest>?> GetAllAsync()
        {
            var newList = new List<Guest>();

            using (var reader = File.OpenText("Data/DataFiles/Dogs.csv"))
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
                               Gender = (Gender)Enum.Parse(typeof(Gender), split[3]),
                               CoatColor = split[4],
                               DateOfBirth = DateTime.Parse(split[5]),
                               FavoriteToy = split[6],
                               EarFloppiness = (EarFloppiness)Enum.Parse(typeof(EarFloppiness), split[7]),
                               SpecialRequests = split[8],
                               IsArchived = bool.Parse(split[9])
                           };
                newList.AddRange(data);
            }
            Guest.CalculateNextId(newList);
            return newList;
        }
    }
}
