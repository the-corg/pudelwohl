﻿using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for the Cuisine tab
    /// </summary>
    public class MealOptionsViewModel : ViewModelBase
    {
        #region Private fields

        private MealOptionViewModel? _selectedMealOption;
        private readonly IMealDataService _mealDataService;
        private readonly IMealOptionDialogService _mealOptionDialogService;
        private readonly IMessageService _messageService;
        private readonly List<MealSelectionViewModel> _selectors = new();
        #endregion


        #region Constructor

        public MealOptionsViewModel(IMealDataService mealDataService, 
            IMealOptionDialogService mealOptionDialogService, IMessageService messageService)
        {
            _mealDataService = mealDataService;
            _mealOptionDialogService = mealOptionDialogService;
            _messageService = messageService;
            MealOptions = _mealDataService.MealOptions;
            MealOptionCollectionView = _mealDataService.SortedMealOptions;
            AddCommand = new DelegateCommand(execute => Add());
            EditCommand = new DelegateCommand(execute => Edit(), canExecute => CanEdit());
            RemoveCommand = new DelegateCommand(execute => Remove(), canExecute => CanRemove());

            BreakfastViewModel = new("Breakfast", _mealDataService, _mealDataService.MealOptionsForBreakfast, 0);
            LunchViewModel = new("Lunch", _mealDataService, _mealDataService.MealOptionsForLunch, 3);
            SnackViewModel = new("Snack", _mealDataService, _mealDataService.MealOptionsForSnack, 6);
            DinnerViewModel = new("Dinner", _mealDataService, _mealDataService.MealOptionsForDinner, 9);
            _selectors.Add(BreakfastViewModel);
            _selectors.Add(LunchViewModel);
            _selectors.Add(SnackViewModel);
            _selectors.Add(DinnerViewModel);
            _mealDataService.DailyMenuUpdated = UpdateSelectors;
        }
        #endregion


        #region Public properties

        /// <summary>
        /// Date for the menu, selected by the user
        /// </summary>
        public DateOnly MenuDate
        {
            get => _mealDataService.MenuDate;
            set
            {
                if (_mealDataService.MenuDate == value)
                    return;

                _mealDataService.MenuDate = value;
                OnPropertyChanged();
                UpdateSelectors();
            }
        }

        /// <summary>
        /// The sorted collection of meal options
        /// </summary>
        public ListCollectionView MealOptionCollectionView { get; }

        /// <summary>
        /// The original collection of meal options
        /// </summary>
        public ObservableCollection<MealOptionViewModel> MealOptions { get; }

        /// <summary>
        /// Currently selected meal option
        /// </summary>
        public MealOptionViewModel? SelectedMealOption
        {
            get => _selectedMealOption;
            set
            {
                if (_selectedMealOption == value)
                    return;

                _selectedMealOption = value;
                OnPropertyChanged();
                RemoveCommand.OnCanExecuteChanged();
                EditCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// View model for breakfast selection
        /// </summary>
        public MealSelectionViewModel BreakfastViewModel { get; }

        /// <summary>
        /// View model for lunch selection
        /// </summary>
        public MealSelectionViewModel LunchViewModel { get; }

        /// <summary>
        /// View model for snack selection
        /// </summary>
        public MealSelectionViewModel SnackViewModel { get; }

        /// <summary>
        /// View model for dinner selection
        /// </summary>
        public MealSelectionViewModel DinnerViewModel { get; }

        /// <summary>
        /// Command for adding a meal option
        /// </summary>
        public DelegateCommand AddCommand { get; }

        /// <summary>
        /// Command for editing a meal option
        /// </summary>
        public DelegateCommand EditCommand { get; }

        /// <summary>
        /// Command for removing a meal option
        /// </summary>
        public DelegateCommand RemoveCommand { get; }

        #endregion


        #region Private helper methods
        private void Add()
        {
            _mealOptionDialogService.ShowMealOptionDialog("New Meal Option");
        }

        private bool CanEdit() => SelectedMealOption is not null;
        private void Edit()
        {
            if (SelectedMealOption is null)
                return;

            _mealOptionDialogService.ShowMealOptionDialog("Edit Meal Option", SelectedMealOption);
        }

        private bool CanRemove() => SelectedMealOption is not null;
        private void Remove()
        {
            if (SelectedMealOption is null)
                return;

            // Count how many times this was selected for daily menus and guests today or in the future
            var today = DateOnly.FromDateTime(DateTime.Now);
            int numberOfDailyMenus = _mealDataService.DailyMenus.Where(
                x => x.Key >= today && x.Value.Menu.Contains(SelectedMealOption.Id)).Count();
            int numberOfGuestMenus = _mealDataService.GuestMenus.Where(
                x => x.Key.Item1 >= today && (x.Value.Breakfast == SelectedMealOption.Id || x.Value.Lunch == SelectedMealOption.Id || 
                x.Value.Snack == SelectedMealOption.Id || x.Value.Dinner == SelectedMealOption.Id)).Count();

            if (numberOfDailyMenus > 0 || numberOfGuestMenus > 0)
            {
                string message = $"This meal option is selected for {numberOfDailyMenus} active daily menu" + 
                    (numberOfDailyMenus != 1 ? "s" : "") + 
                    $" and {numberOfGuestMenus} active guest menu" + (numberOfGuestMenus != 1 ? "s" : "") +
                    $".\n\nAre you sure you want to delete the meal option?";

                if (!_messageService.ShowConfirmation(message))
                    return;

                _mealDataService.RemoveMealOptionFromMenus(SelectedMealOption.Id, true, true, true, true);
            }

            MealOptions.Remove(SelectedMealOption);
            SelectedMealOption = null;
        }

        private void UpdateSelectors()
        {
            foreach (var selector in _selectors)
                selector.UpdateOptions();
        }
        #endregion

    }
}
