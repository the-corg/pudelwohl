using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionsViewModel : ViewModelBase
    {
        private MealOptionViewModel? _selectedMealOption;
        private readonly IMealDataService _mealDataService;
        private readonly IMealOptionDialogService _mealOptionDialogService;
        private readonly IMessageService _messageService;
        private readonly List<MealSelectionViewModel> _selectors = new();

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

        public ListCollectionView MealOptionCollectionView { get; }
        public ObservableCollection<MealOptionViewModel> MealOptions { get; }

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

        public MealSelectionViewModel BreakfastViewModel { get; }
        public MealSelectionViewModel LunchViewModel { get; }
        public MealSelectionViewModel SnackViewModel { get; }
        public MealSelectionViewModel DinnerViewModel { get; }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand RemoveCommand { get; }


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

    }
}
