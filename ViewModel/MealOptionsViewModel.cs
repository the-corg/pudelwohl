using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionsViewModel : ViewModelBase
    {
        private MealOptionViewModel? _selectedMealOption;
        private readonly MainViewModel _mainViewModel;

        public MealOptionsViewModel(MainViewModel mainViewModel)
        {
            MealOptions = mainViewModel.MealOptions;
            AddCommand = new DelegateCommand(execute => Add());
            EditCommand = new DelegateCommand(execute => Edit(), canExecute => CanEdit());
            RemoveCommand = new DelegateCommand(execute => Remove(), canExecute => CanRemove());
            _mainViewModel = mainViewModel;
        }

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
            var mealOption = new MealOption { Name = "NEW MEAL OPTION" };
            var viewModel = new MealOptionViewModel(mealOption, _mainViewModel);
            MealOptions.Add(viewModel);
            SelectedMealOption = viewModel;
        }

        private bool CanEdit() => SelectedMealOption is not null;
        private void Edit()
        {
            if (SelectedMealOption is null)
                return;

            SelectedMealOption.Name = "!" + SelectedMealOption.Name;
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
