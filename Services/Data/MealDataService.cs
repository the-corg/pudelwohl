using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public interface IMealDataService
    {
        ObservableCollection<MealOptionViewModel> MealOptions { get; }
        ListCollectionView SortedMealOptions { get; }
        //DateOnly MenuDate { get; set; }
        Task LoadAsync();
    }
    public class MealDataService : BaseDataService, IMealDataService
    {
        private readonly IMealOptionDataProvider _mealOptionDataProvider;

        public MealDataService(IMealOptionDataProvider mealOptionDataProvider)
        {
            _mealOptionDataProvider = mealOptionDataProvider;
            SortedMealOptions = new ListCollectionView(MealOptions);
            SortedMealOptions.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();
        public ListCollectionView SortedMealOptions { get; }

        // Used in MealOptionsViewModel for Binding with the DatePicker above the daily menu
        public DateOnly MenuDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public async Task LoadAsync()
        {
            var mealOptions = await _mealOptionDataProvider.GetAllAsync();
            LoadCollection(MealOptions, mealOptions, mealOption => new MealOptionViewModel(mealOption, this));
        }

    }
}
