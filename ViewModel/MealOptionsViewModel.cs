using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionsViewModel : ViewModelBase
    {
        private MealOptionViewModel? _selectedMealOption;
        private readonly MainViewModel _mainViewModel;

        public MealOptionsViewModel(ObservableCollection<MealOptionViewModel> mealOptions, MainViewModel mainViewModel)
        {
            MealOptions = mealOptions;
            AddCommand = new DelegateCommand(Add);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            RemoveCommand = new DelegateCommand(Remove, CanRemove);
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


        private void Add(object? parameter)
        {
            var mealOption = new MealOption { Name = "NEW MEAL OPTION" };
            var viewModel = new MealOptionViewModel(mealOption, _mainViewModel);
            MealOptions.Add(viewModel);
            SelectedMealOption = viewModel;
        }

        private void Edit(object? parameter)
        {
            if (SelectedMealOption is null)
                return;

            SelectedMealOption.Name = "!" + SelectedMealOption.Name;
        }

        private bool CanEdit(object? parameter) => SelectedMealOption is not null;

        private void Remove(object? parameter)
        {
            if (SelectedMealOption is null)
                return;

            MealOptions.Remove(SelectedMealOption);
            SelectedMealOption = null;
        }

        private bool CanRemove(object? parameter) => SelectedMealOption is not null;

    }
}
