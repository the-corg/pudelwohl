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
        ListCollectionView MealOptionsForBreakfast { get; }
        ListCollectionView MealOptionsForLunch { get; }
        ListCollectionView MealOptionsForSnack { get; }
        ListCollectionView MealOptionsForDinner { get; }
        Dictionary<DateOnly, DailyMenu> DailyMenus { get; }
        Dictionary<DateOnly, Dictionary<int, GuestMenu>> GuestMenus { get; }
        IMessageService MessageService { get; }
        DateOnly MenuDate { get; set; }
        DailyMenu? DailyMenuForSelectedDate { get; }
        Action? DailyMenuUpdated { get; set; }
        Task LoadAsync();
    }
    public class MealDataService : BaseDataService, IMealDataService
    {
        private readonly IMealOptionDataProvider _mealOptionDataProvider;
        private readonly IDailyMenuDataProvider _dailyMenuDataProvider;
        private readonly IGuestMenuDataProvider _guestMenuDataProvider;

        private DateOnly _menuDate = DateOnly.FromDateTime(DateTime.Now);
        private DailyMenu? _dailyMenuForSelectedDate;

        public MealDataService(IMealOptionDataProvider mealOptionDataProvider, IDailyMenuDataProvider dailyMenuDataProvider,
            IGuestMenuDataProvider guestMenuDataProvider, IMessageService messageService)
        {
            _mealOptionDataProvider = mealOptionDataProvider;
            _dailyMenuDataProvider = dailyMenuDataProvider;
            _guestMenuDataProvider = guestMenuDataProvider;
            MessageService = messageService;

            SortedMealOptions = new ListCollectionView(MealOptions);
            SortedMealOptions.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            MealOptionsForBreakfast = new ListCollectionView(MealOptions);
            MealOptionsForBreakfast.Filter = option => ((MealOptionViewModel)option).IsBreakfast;
            MealOptionsForBreakfast.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            MealOptionsForLunch = new ListCollectionView(MealOptions);
            MealOptionsForLunch.Filter = option => ((MealOptionViewModel)option).IsLunch;
            MealOptionsForLunch.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            MealOptionsForSnack = new ListCollectionView(MealOptions);
            MealOptionsForSnack.Filter = option => ((MealOptionViewModel)option).IsSnack;
            MealOptionsForSnack.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            MealOptionsForDinner = new ListCollectionView(MealOptions);
            MealOptionsForDinner.Filter = option => ((MealOptionViewModel)option).IsDinner;
            MealOptionsForDinner.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();
        public ListCollectionView SortedMealOptions { get; }

        public ListCollectionView MealOptionsForBreakfast { get; }
        public ListCollectionView MealOptionsForLunch { get; }
        public ListCollectionView MealOptionsForSnack { get; }
        public ListCollectionView MealOptionsForDinner { get; }

        public Dictionary<DateOnly, DailyMenu> DailyMenus { get; } = new();
        public Dictionary<DateOnly, Dictionary<int, GuestMenu>> GuestMenus { get; } = new();

        public IMessageService MessageService { get; }

        // Used in MealOptionsViewModel for Binding with the DatePicker above the daily menu
        public DateOnly MenuDate
        {
            get => _menuDate;
            set
            {
                _menuDate = value;
                if (!DailyMenus.TryGetValue(_menuDate, out _dailyMenuForSelectedDate))
                {
                    _dailyMenuForSelectedDate = new DailyMenu() 
                    {
                        Date = MenuDate,
                        Menu = new int[12]
                    };
                    DailyMenus.Add(_menuDate, _dailyMenuForSelectedDate);
                }
            }
        }

        public DailyMenu? DailyMenuForSelectedDate => _dailyMenuForSelectedDate;

        public Action? DailyMenuUpdated { get; set; }

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
            // Calling local setter after loading all daily menus - initializes the daily menu for the selected date
            MenuDate = DateOnly.FromDateTime(DateTime.Now);
            MealOptions.CollectionChanged += (s, e) => DailyMenuUpdated?.Invoke();
            DailyMenuUpdated?.Invoke();

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
