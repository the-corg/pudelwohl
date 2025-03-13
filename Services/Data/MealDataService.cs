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
        Dictionary<(DateOnly, int), GuestMenu> GuestMenus { get; }
        DateOnly MenuDate { get; set; }
        DailyMenu? DailyMenuForSelectedDate { get; }
        Action? DailyMenuUpdated { get; set; }
        Action? GuestMenuUpdated { get; set; }
        MealOptionViewModel? GetMealOptionById(int id);
        void RemoveMealOptionFromMenus(int mealOptionId, bool deleteBreakfast, bool deleteLunch, bool deleteSnack, bool deleteDinner);
        void UpdateMenus();
        Task LoadAsync();
        Task SaveDataAsync();
        void DebouncedSave();
    }
    public class MealDataService : BaseDataService, IMealDataService
    {
        private readonly IMealOptionDataProvider _mealOptionDataProvider;
        private readonly IDailyMenuDataProvider _dailyMenuDataProvider;
        private readonly IGuestMenuDataProvider _guestMenuDataProvider;

        private DateOnly _menuDate = DateOnly.FromDateTime(DateTime.Now);
        private DailyMenu? _dailyMenuForSelectedDate;

        private readonly SemaphoreSlim _saveLock = new(1, 1);

        public MealDataService(IMealOptionDataProvider mealOptionDataProvider, IDailyMenuDataProvider dailyMenuDataProvider,
            IGuestMenuDataProvider guestMenuDataProvider, IMessageService messageService)
        {
            _mealOptionDataProvider = mealOptionDataProvider;
            _dailyMenuDataProvider = dailyMenuDataProvider;
            _guestMenuDataProvider = guestMenuDataProvider;

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
        public Dictionary<(DateOnly, int), GuestMenu> GuestMenus { get; } = new();

        protected override CancellationTokenSource DebounceCts { get; set; } = new();

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
        public Action? GuestMenuUpdated { get; set; }

        public MealOptionViewModel? GetMealOptionById(int id)
        {
            return MealOptions.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveMealOptionFromMenus(int mealOptionId, bool deleteBreakfast, bool deleteLunch, bool deleteSnack, bool deleteDinner)
        {
            foreach (var menu in DailyMenus.Values)
            {
                for (int i = 0; i < 12; i++)
                {
                    if ((menu.Menu[i] == mealOptionId) && 
                        ((deleteBreakfast && i < 3) || (deleteLunch && i >= 3 && i < 6) ||
                        (deleteSnack && i >= 6 && i < 9) || (deleteDinner && i >= 9))) 
                        menu.Menu[i] = 0;
                }
            }

            foreach (var menu in GuestMenus.Values)
            {
                if (deleteBreakfast && menu.Breakfast == mealOptionId)
                    menu.Breakfast = 0;
                if (deleteLunch && menu.Lunch == mealOptionId)
                    menu.Lunch = 0;
                if (deleteSnack && menu.Snack == mealOptionId)
                    menu.Snack = 0;
                if (deleteDinner && menu.Dinner == mealOptionId)
                    menu.Dinner = 0;
            }
        }

        public void UpdateMenus()
        {
            DailyMenuUpdated?.Invoke();
            GuestMenuUpdated?.Invoke();
        }

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
                    GuestMenus.Add((guestMenu.Date, guestMenu.GuestId), guestMenu);
            }

            // Calling local setter after loading all daily menus - initializes the daily menu for the selected date
            MenuDate = DateOnly.FromDateTime(DateTime.Now);
            MealOptions.CollectionChanged += (s, e) => UpdateMenus();
            UpdateMenus();
        }

        public override async Task SaveDataAsync()
        {
            // Prevent multiple saves from running at the same time
            await _saveLock.WaitAsync();
            try
            {
                // Cancel any pending debounced save
                DebounceCts.Cancel();

                // TODO await _guestDataProvider.SaveAsync(Guests.Select(x => x.GetGuest()));
            }
            finally
            {
                _saveLock.Release();
            }
        }

    }
}
