using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealSelectionViewModel : ViewModelBase
    {
        private readonly IMealDataService _mealDataService;
        private readonly int _menuIndexOffset;
        private MealOptionViewModel?[] _selectedOption = new MealOptionViewModel?[3];

        public MealSelectionViewModel(string mealName, IMealDataService mealDataService, ListCollectionView mealOptions, int menuIndexOffset)
        {
            MealName = mealName;
            _mealDataService = mealDataService;
            MealOptions = mealOptions;
            _menuIndexOffset = menuIndexOffset;
        }

        public string MealName { get; }
        public ListCollectionView MealOptions { get; }

        public MealOptionViewModel? SelectedOption1
        {
            get => _selectedOption[0];
            set
            {
                int i = 0;
                if (_selectedOption[i] == value)
                    return;

                _selectedOption[i] = value;
                if (value is not null)
                    ChangeMenu(i, value!.Id);
                OnPropertyChanged();
            }
        }
        public MealOptionViewModel? SelectedOption2
        {
            get => _selectedOption[1];
            set
            {
                int i = 1;
                if (_selectedOption[i] == value)
                    return;

                _selectedOption[i] = value;
                if (value is not null)
                    ChangeMenu(i, value!.Id);
                OnPropertyChanged();
            }
        }
        public MealOptionViewModel? SelectedOption3
        {
            get => _selectedOption[2];
            set
            {
                int i = 2;
                if (_selectedOption[i] == value)
                    return;

                _selectedOption[i] = value;
                if (value is not null)
                    ChangeMenu(i, value!.Id);
                OnPropertyChanged();
            }
        }

        public void UpdateOptions()
        {
            for (int i = 0; i < 3; i++)
            {
                var mealOptionId = _mealDataService.DailyMenuForSelectedDate?.Menu[_menuIndexOffset + i];
                if (mealOptionId is null)
                    continue;
                _selectedOption[i] = _mealDataService.GetMealOptionById((int)mealOptionId);
            }
            OnPropertyChanged(nameof(SelectedOption1));
            OnPropertyChanged(nameof(SelectedOption2));
            OnPropertyChanged(nameof(SelectedOption3));
        }

        private void ChangeMenu(int optionNumber, int id)
        {
            var menu = _mealDataService.DailyMenuForSelectedDate;
            if (menu is not null)
            {
                menu.Menu[_menuIndexOffset + optionNumber] = id;
            }
            _mealDataService.UpdateMenus();
        }

    }
}
