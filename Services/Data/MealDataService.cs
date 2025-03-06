using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public interface IMealDataService
    {
        ObservableCollection<MealOptionViewModel> MealOptions { get; }
        ListCollectionView SortedMealOptions { get; }
        Dictionary<DateOnly, DailyMenu> DailyMenus { get; }
        Dictionary<DateOnly, Dictionary<int, GuestMenu>> GuestMenus { get; }
        IMessageService MessageService { get; }
        DateOnly MenuDate { get; set; }
        Task LoadAsync();
    }
    public class MealDataService : BaseDataService, IMealDataService
    {
        private readonly IMealOptionDataProvider _mealOptionDataProvider;
        private readonly IDailyMenuDataProvider _dailyMenuDataProvider;
        private readonly IGuestMenuDataProvider _guestMenuDataProvider;

        public MealDataService(IMealOptionDataProvider mealOptionDataProvider, IDailyMenuDataProvider dailyMenuDataProvider,
            IGuestMenuDataProvider guestMenuDataProvider, IMessageService messageService)
        {
            _mealOptionDataProvider = mealOptionDataProvider;
            _dailyMenuDataProvider = dailyMenuDataProvider;
            _guestMenuDataProvider = guestMenuDataProvider;
            MessageService = messageService;
            SortedMealOptions = new ListCollectionView(MealOptions);
            SortedMealOptions.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();
        public ListCollectionView SortedMealOptions { get; }
        public Dictionary<DateOnly, DailyMenu> DailyMenus { get; } = new();
        public Dictionary<DateOnly, Dictionary<int, GuestMenu>> GuestMenus { get; } = new();

        public IMessageService MessageService { get; }

        // Used in MealOptionsViewModel for Binding with the DatePicker above the daily menu
        public DateOnly MenuDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public async Task LoadAsync()
        {
            var mealOptions = await _mealOptionDataProvider.GetAllAsync();
            LoadCollection(MealOptions, mealOptions, mealOption => new MealOptionViewModel(mealOption));

            var dailyMenuList = await _dailyMenuDataProvider.GetAllAsync();
            if (DailyMenus.Count == 0 && dailyMenuList is not null)
            {
                foreach (var dailyMenu in dailyMenuList)
                    DailyMenus.Add(dailyMenu.Date, dailyMenu);
            }

            var guestMenuList = await _guestMenuDataProvider.GetAllAsync();
            if (GuestMenus.Count == 0 && guestMenuList is not null)
            {
                foreach (var guestMenu in guestMenuList)
                {
                    if (!GuestMenus.TryGetValue(guestMenu.Date, out Dictionary<int, GuestMenu>? menusForDate))
                    { 
                        menusForDate = new Dictionary<int, GuestMenu>();
                        GuestMenus.Add(guestMenu.Date, menusForDate);
                    }

                    try
                    {
                        menusForDate.Add(guestMenu.GuestId, guestMenu);
                    }
                    catch (ArgumentException)
                    {
                        MessageService.ShowMessage("Error: Duplicate guest menus for the same date in the input data.");
                    }
                }
            }
        }

    }
}
