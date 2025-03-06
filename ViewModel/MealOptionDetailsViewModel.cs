using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionDetailsViewModel : ViewModelBase
    {
        private readonly string _headerText;
        private readonly IMealDataService _mealDataService;

        private MealOptionViewModel? _mealOption;
        private string _mealOptionName = "";
        private bool _isBreakfast;
        private bool _isLunch;
        private bool _isSnack;
        private bool _isDinner;

        public MealOptionDetailsViewModel(IMealDataService mealDataService, string headerText, MealOptionViewModel? mealOption)
        {
            _mealDataService = mealDataService;
            _headerText = headerText;
            _mealOption = mealOption;
            if (mealOption is not null )
            {
                // This is an Edit
                _mealOptionName = mealOption.Name!;
                _isBreakfast = mealOption.IsBreakfast;
                _isLunch = mealOption.IsLunch;
                _isSnack = mealOption.IsSnack;
                _isDinner = mealOption.IsDinner;
            }

            ConfirmCommand = new DelegateCommand(execute => Confirm(), canExecute => CanConfirm());
        }

        public Action? CloseOnConfirmAction { get; set; } // Delegate for closing the window

        public string HeaderText => _headerText;

        public string MealOptionName
        {
            get => _mealOptionName;
            set
            {
                _mealOptionName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public bool IsBreakfast
        {
            get => _isBreakfast;
            set
            {
                _isBreakfast = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public bool IsLunch
        {
            get => _isLunch;
            set
            {
                _isLunch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public bool IsSnack
        {
            get => _isSnack;
            set
            {
                _isSnack = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public bool IsDinner
        {
            get => _isDinner;
            set
            {
                _isDinner = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public DelegateCommand ConfirmCommand { get; }

        public string ButtonDisabledReason
        {
            get
            {
                string result = "";
                if (string.IsNullOrEmpty(MealOptionName))
                    result += "The name cannot be empty\n\n";
                if (!(IsBreakfast || IsLunch || IsSnack || IsDinner))
                    result += "Please select at least one of the checkboxes\n\n";

                if (result.Length > 0)
                    result = result[..^2];
                return result;
            }
        }

        // Make the Confirm button inactive if the name is empty
        private bool CanConfirm() => (!string.IsNullOrEmpty(MealOptionName)) && (IsBreakfast || IsLunch || IsSnack || IsDinner);
        private void Confirm()
        {
            // Race condition check - UI is not guaranteed to check CanConfirm immediately before Confirm
            if (!CanConfirm())
            {
                ConfirmCommand.OnCanExecuteChanged();
                return;
            }

            if (_mealOption is not null)
            {
                // To easily refresh all derived collection views
                _mealDataService.MealOptions.Remove(_mealOption);
            }

            _mealOption ??= new MealOptionViewModel(new MealOption() { Name = "" });

            _mealOption.Name = MealOptionName;
            _mealOption.IsBreakfast = IsBreakfast;
            _mealOption.IsLunch = IsLunch;
            _mealOption.IsSnack = IsSnack;
            _mealOption.IsDinner = IsDinner;

            _mealDataService.MealOptions.Add(_mealOption);

            // Close the dialog
            CloseOnConfirmAction?.Invoke();
        }


    }
}
