using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for the Meal Option Details dialog (for editing and adding meal options)
    /// </summary>
    public class MealOptionDetailsViewModel : ViewModelBase
    {
        #region Private fields

        private readonly string _headerText;
        private readonly IMealDataService _mealDataService;

        private MealOptionViewModel? _mealOption;
        private string _mealOptionName = "";
        private bool _isBreakfast;
        private bool _isLunch;
        private bool _isSnack;
        private bool _isDinner;
        #endregion


        #region Constructor

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
        #endregion


        #region Public properties

        /// <summary>
        /// Delegate for closing the dialog window on Confirm
        /// </summary>
        public Action? CloseOnConfirmAction { get; set; }

        /// <summary>
        /// Header text for the dialog
        /// </summary>
        public string HeaderText => _headerText;

        /// <summary>
        /// Name of the meal option
        /// </summary>
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

        /// <summary>
        /// Shows whether the meal option can be selected for breakfast
        /// </summary>
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

        /// <summary>
        /// Shows whether the meal option can be selected for lunch
        /// </summary>
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

        /// <summary>
        /// Shows whether the meal option can be selected for snack
        /// </summary>
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

        /// <summary>
        /// Shows whether the meal option can be selected for dinner
        /// </summary>
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

        /// <summary>
        /// Command to confirm the meal option
        /// </summary>
        public DelegateCommand ConfirmCommand { get; }

        /// <summary>
        /// Text that explains the reason why the Confirm button is disabled
        /// </summary>
        public string? ButtonDisabledReason
        {
            get
            {
                string result = "";
                if (string.IsNullOrEmpty(MealOptionName))
                    result += "The name cannot be empty\n\n";
                if (!(IsBreakfast || IsLunch || IsSnack || IsDinner))
                    result += "Please select at least one of the checkboxes\n\n";

                return result.Length > 0 ? result[..^2] : null;
            }
        }
        #endregion


        #region Private methods (for the Confirm command)

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
                // If it was allowed for some of the meals and not allowed anymore, remove it from menus for these meals
                _mealDataService.RemoveMealOptionFromMenus(_mealOption.Id, _mealOption.IsBreakfast && !IsBreakfast,
                    _mealOption.IsLunch && !IsLunch, _mealOption.IsSnack && !IsSnack, _mealOption.IsDinner && !IsDinner);
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
        #endregion

    }
}
