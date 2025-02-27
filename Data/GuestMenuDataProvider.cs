﻿using System.IO;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data
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

            using (var reader = File.OpenText("Data/DataFiles/Dogs and Menu Options.csv"))
            {
                var fileText = await reader.ReadToEndAsync();
                var lines = fileText.Split(Environment.NewLine);

                var data = from line in lines.Skip(1)
                           let split = line.Split('|')
                           select new GuestMenu
                           {
                               Date = DateTime.Parse(split[0]),
                               GuestId = int.Parse(split[1]),
                               Breakfast = split[2],
                               Lunch = split[3],
                               Snack = split[4],
                               Dinner = split[5]
                           };
                newList.AddRange(data);
            }
            return newList;
        }
    }
}

