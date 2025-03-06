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

        public MealOptionsViewModel(IMealDataService mealDataService, IMealOptionDialogService mealOptionDialogService)
        {
            _mealDataService = mealDataService;
            _mealOptionDialogService = mealOptionDialogService;
            MealOptions = _mealDataService.MealOptions;
            MealOptionCollectionView = _mealDataService.SortedMealOptions;
            AddCommand = new DelegateCommand(execute => Add());
            EditCommand = new DelegateCommand(execute => Edit(), canExecute => CanEdit());
            RemoveCommand = new DelegateCommand(execute => Remove(), canExecute => CanRemove());
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

            // TODO: Check if this was selected by a guest today or on a future date and ask the user
            // don't ask about past meals, they are unimportant

            MealOptions.Remove(SelectedMealOption);
            SelectedMealOption = null;
        }

    }
}
