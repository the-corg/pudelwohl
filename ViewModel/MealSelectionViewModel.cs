using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealSelectionViewModel : ViewModelBase
    {
        private readonly IMealDataService _mealDataService;
        private string? _selectedOption1;
        private string? _selectedOption2;
        private string? _selectedOption3;

        public MealSelectionViewModel(string mealName, IMealDataService mealDataService)
        {
            MealName = mealName;
            _mealDataService = mealDataService;
        }

        public string MealName { get; }
        public ObservableCollection<string> MealOptions { get; } = new() { "Option 1", "Option 2" };
        
        public string? SelectedOption1
        {
            get => _selectedOption1;
            set { 
                _selectedOption1 = value;
                OnPropertyChanged(); 
            }
        }

        public string? SelectedOption2
        {
            get => _selectedOption2;
            set { 
                _selectedOption2 = value; 
                OnPropertyChanged(); 
            }
        }

        public string? SelectedOption3
        {
            get => _selectedOption3;
            set { 
                _selectedOption3 = value; 
                OnPropertyChanged(); 
            }
        }

    }
}
