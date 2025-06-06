﻿using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    /// <summary>
    /// Manages all data related to meals
    /// </summary>
    public interface IMealDataService
    {
        /// <summary>
        /// The collection of meal options
        /// </summary>
        ObservableCollection<MealOptionViewModel> MealOptions { get; }

        /// <summary>
        /// Sorted view for the collection of meal options
        /// </summary>
        ListCollectionView SortedMealOptions { get; }

        /// <summary>
        /// Sorted and filtered view for the collections of meal options,
        /// filtering out anything except breakfast meal options
        /// </summary>
        ListCollectionView MealOptionsForBreakfast { get; }

        /// <summary>
        /// Sorted and filtered view for the collections of meal options,
        /// filtering out anything except lunch meal options
        /// </summary>
        ListCollectionView MealOptionsForLunch { get; }

        /// <summary>
        /// Sorted and filtered view for the collections of meal options,
        /// filtering out anything except snack meal options
        /// </summary>
        ListCollectionView MealOptionsForSnack { get; }

        /// <summary>
        /// Sorted and filtered view for the collections of meal options,
        /// filtering out anything except dinner meal options
        /// </summary>
        ListCollectionView MealOptionsForDinner { get; }

        /// <summary>
        /// The dictionary of daily menus with dates as keys
        /// </summary>
        Dictionary<DateOnly, DailyMenu> DailyMenus { get; }

        /// <summary>
        /// The dictionary of menus selected for a guest
        /// with the key consisting of the date and the guest's ID
        /// </summary>
        Dictionary<(DateOnly, int), GuestMenu> GuestMenus { get; }

        /// <summary>
        /// Currently selected date for the menu on the Cuisine tab
        /// (used in MealOptionsViewModel for Binding with the DatePicker above the daily menu)
        /// </summary>
        DateOnly MenuDate { get; set; }

        /// <summary>
        /// The daily menu for the selected date on the Cuisine tab
        /// </summary>
        DailyMenu? DailyMenuForSelectedDate { get; }

        /// <summary>
        /// The delegate to be invoked when data for a daily menu has been updated
        /// </summary>
        Action? DailyMenuUpdated { get; set; }

        /// <summary>
        /// The delegate to be invoked when data for a guest menu has been updated
        /// </summary>
        Action? GuestMenuUpdated { get; set; }

        /// <summary>
        /// Returns a meal option with the ID equal to <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID of the meal option</param>
        /// <returns>The meal option with the ID equal to <paramref name="id"/>, or <c>null</c>, if not found</returns>
        MealOptionViewModel? GetMealOptionById(int id);

        /// <summary>
        /// Removes a meal option from DailyMenus and GuestMenus,
        /// independently for each type of meal
        /// </summary>
        /// <param name="mealOptionId">ID of the meal option</param>
        /// <param name="deleteBreakfast">True, if it should be removed from breakfast menus<br/>False, otherwise</param>
        /// <param name="deleteLunch">True, if it should be removed from lunch menus<br/>False, otherwise</param>
        /// <param name="deleteSnack">True, if it should be removed from snack menus<br/>False, otherwise</param>
        /// <param name="deleteDinner">True, if it should be removed from dinner menus<br/>False, otherwise</param>
        void RemoveMealOptionFromMenus(int mealOptionId, bool deleteBreakfast, bool deleteLunch, bool deleteSnack, bool deleteDinner);

        /// <summary>
        /// To be called after some menu data has changed (refreshes collections and saves the data)
        /// </summary>
        void UpdateMenus();

        /// <summary>
        /// Loads all the data managed by the data service from the corresponding data providers
        /// </summary>
        Task LoadAsync();

        /// <summary>
        /// Saves all the data managed by the data service asynchronously
        /// (as soon as possible)
        /// </summary>
        Task SaveDataAsync();

        /// <summary>
        /// Saves all the data managed by the data service asynchronously,
        /// but only if no new save calls arrive within <c>_debounceTime</c>.
        /// </summary>
        void DebouncedSave();
    }
    public class MealDataService : BaseDataService, IMealDataService
    {

        #region Private fields

        private readonly IMealOptionDataProvider _mealOptionDataProvider;
        private readonly IDailyMenuDataProvider _dailyMenuDataProvider;
        private readonly IGuestMenuDataProvider _guestMenuDataProvider;

        private DateOnly _menuDate = DateOnly.FromDateTime(DateTime.Now);
        private DailyMenu? _dailyMenuForSelectedDate;

        #endregion


        #region Constructor

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
        #endregion


        #region Public properties (see interface)

        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();
        public ListCollectionView SortedMealOptions { get; }

        public ListCollectionView MealOptionsForBreakfast { get; }
        public ListCollectionView MealOptionsForLunch { get; }
        public ListCollectionView MealOptionsForSnack { get; }
        public ListCollectionView MealOptionsForDinner { get; }

        public Dictionary<DateOnly, DailyMenu> DailyMenus { get; } = new();
        public Dictionary<(DateOnly, int), GuestMenu> GuestMenus { get; } = new();

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

        #endregion


        #region Public methods (see interface)

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
            DebouncedSave();
        }

        public async Task LoadAsync()
        {
            var mealOptions = await _mealOptionDataProvider.LoadAsync();
            if (mealOptions is null)
                return;
            IdGenerator.Instance.CalculateMaxId(mealOptions);
            LoadCollection(MealOptions, mealOptions, mealOption => new MealOptionViewModel(mealOption));

            var dailyMenuList = await _dailyMenuDataProvider.LoadAsync();
            if (DailyMenus.Count == 0 && dailyMenuList is not null)
            {
                foreach (var dailyMenu in dailyMenuList)
                    DailyMenus.Add(dailyMenu.Date, dailyMenu);
            }

            var guestMenuList = await _guestMenuDataProvider.LoadAsync();
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
        #endregion


        #region Protected method used by the base class for saving the data

        protected override async Task SaveCollectionsAsync()
        {
            await _mealOptionDataProvider.SaveAsync(MealOptions.Select(x => x.GetMealOption()));
            await _dailyMenuDataProvider.SaveAsync(DailyMenus.Values);
            await _guestMenuDataProvider.SaveAsync(GuestMenus.Values);
        }
        #endregion

    }
}
