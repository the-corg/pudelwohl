using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealSelectionViewModel : ViewModelBase
    {
        private readonly IMealDataService _mealDataService;
        private readonly int _menuIndexOffset;

        public MealSelectionViewModel(string mealName, IMealDataService mealDataService, ListCollectionView mealOptions, int menuIndexOffset)
        {
            MealName = mealName;
            _mealDataService = mealDataService;
            MealOptions = mealOptions;
            _menuIndexOffset = menuIndexOffset;
        }

        public string MealName { get; }
        public ListCollectionView MealOptions { get; }
        
        public string? SelectedOption1
        {
            get => GetSelectedOption(1);
            set {
                if (GetSelectedOption(1) == value)
                    return;

                SetSelectedOption(1, value);
                OnPropertyChanged(); 
            }
        }

        public string? SelectedOption2
        {
            get => GetSelectedOption(2);
            set
            {
                if (GetSelectedOption(2) == value)
                    return;

                SetSelectedOption(2, value);
                OnPropertyChanged();
            }
        }

        public string? SelectedOption3
        {
            get => GetSelectedOption(3);
            set
            {
                if (GetSelectedOption(3) == value)
                    return;

                SetSelectedOption(3, value);
                OnPropertyChanged();
            }
        }

        private string? GetSelectedOption(int optionNumber)
        {
            var mealOptionId = _mealDataService.DailyMenuForSelectedDate?.Menu[_menuIndexOffset + optionNumber - 1];
            return mealOptionId is null ? null :_mealDataService.MealOptions.FirstOrDefault(x => x.Id == mealOptionId!)?.NameWithId;
        }

        private void SetSelectedOption(int optionNumber, string? newValue)
        {
            int newIntValue = 0;
            if (newValue is not null)
            {
                newIntValue = int.Parse(newValue.Split("#").Last()[..^1]);
            }
            var menu = _mealDataService.DailyMenuForSelectedDate;
            if (menu is not null)
            {
                menu.Menu[_menuIndexOffset + optionNumber - 1] = newIntValue;
            }
        }

    }
}
