﻿
using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionsViewModel : ViewModelBase
    {
        private readonly IMealOptionDataProvider _mealOptionDataProvider;
        private MealOptionViewModel? _selectedMealOption;

        public MealOptionsViewModel(IMealOptionDataProvider mealOptionDataProvider)
        {
            _mealOptionDataProvider = mealOptionDataProvider;
            AddCommand = new DelegateCommand(Add);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            RemoveCommand = new DelegateCommand(Remove, CanRemove);
        }

        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();

        public MealOptionViewModel? SelectedMealOption
        {
            get => _selectedMealOption;
            set
            {
                _selectedMealOption = value;
                OnPropertyChanged();
                RemoveCommand.OnCanExecuteChanged();
                EditCommand.OnCanExecuteChanged();
            }

        }


        public DelegateCommand AddCommand { get; }

        public DelegateCommand EditCommand { get; }

        public DelegateCommand RemoveCommand { get; }

        public async Task LoadAsync()
        {
            if (MealOptions.Count > 0) 
                return;

            var mealOptions = await _mealOptionDataProvider.GetAllAsync();
            if (mealOptions is not null)
            {
                foreach (var mealOption in mealOptions)
                {
                    MealOptions.Add(new MealOptionViewModel(mealOption));
                }
            }
        }

        private void Add(object? parameter)
        {
            var mealOption = new MealOption { Name = "NEW MEAL OPTION" };
            var viewModel = new MealOptionViewModel(mealOption);
            MealOptions.Add(viewModel);
            SelectedMealOption = viewModel;
        }

        private void Edit(object? parameter)
        {
            if (SelectedMealOption is not null)
            {
                SelectedMealOption.Name = "!" + SelectedMealOption.Name;
            }
        }

        private bool CanEdit(object? parameter) => SelectedMealOption is not null;

        private void Remove(object? parameter)
        {
            if (SelectedMealOption is not null)
            {
                MealOptions.Remove(SelectedMealOption);
                SelectedMealOption = null;
            }
        }

        private bool CanRemove(object? parameter) => SelectedMealOption is not null;

    }
}
