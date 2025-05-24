using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for the meal selection options for a single meal on the Cuisine tab
    /// </summary>
    public class MealSelectionViewModel : ViewModelBase
    {
        #region Private fields and the constructor

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
        #endregion


        #region Public properties

        /// <summary>
        /// Name of the meal (Breakfast, Lunch, Snack, or Dinner)
        /// </summary>
        public string MealName { get; }

        /// <summary>
        /// Collection of available meal options for the current meal
        /// </summary>
        public ListCollectionView MealOptions { get; }

        /// <summary>
        /// First selected meal option 
        /// </summary>
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

        /// <summary>
        /// Second selected meal option
        /// </summary>
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

        /// <summary>
        /// Third selected meal option
        /// </summary>
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
        #endregion


        #region Public method

        /// <summary>
        /// Updates the selected meal options, reloading them from the DailyMenu object for the selected date
        /// </summary>
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
        #endregion


        #region Private helper method

        private void ChangeMenu(int optionNumber, int id)
        {
            var menu = _mealDataService.DailyMenuForSelectedDate;
            if (menu is not null)
            {
                menu.Menu[_menuIndexOffset + optionNumber] = id;
            }
            _mealDataService.UpdateMenus();
        }
        #endregion

    }
}
