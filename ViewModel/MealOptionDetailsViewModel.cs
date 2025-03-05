using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionDetailsViewModel : ViewModelBase
    {
        private readonly string _headerText;
        private readonly string _mealOptionName;
        private readonly string? _initialMealOptionName;
        private readonly IMealDataService _mealDataService;
        private readonly IMessageService _messageService;

        public MealOptionDetailsViewModel(IMealDataService mealDataService, 
            IMessageService messageService, string headerText)
        {
            _mealDataService = mealDataService;
            _messageService = messageService;
            _headerText = headerText;

            ConfirmCommand = new DelegateCommand(execute => Confirm(), canExecute => CanConfirm());
        }

        public Action? CloseOnConfirmAction { get; set; } // Delegate for closing the window

        public string HeaderText => _headerText;

        public string MealOptionName {  get; set; }

        public DelegateCommand ConfirmCommand { get; }

        public string ButtonDisabledReason
        {
            get
            {
                string result = "";
                if (MealOptionName is null)
                    result += "The name cannot be empty";

                return result;
            }
        }

        // Make the Confirm button inactive if the name is empty
        private bool CanConfirm() => MealOptionName is not null;
        private void Confirm()
        {
            // Race condition check - UI is not guaranteed to check CanConfirm immediately before Confirm
            if (MealOptionName is null)
            {
                ConfirmCommand.OnCanExecuteChanged();
                return;
            }

            // TODO:

            // All checks done. Add the mealOption now
            var mealOption = new MealOption()
            {
                Name = MealOptionName
            };

            _mealDataService.MealOptions.Add(new MealOptionViewModel(mealOption, _mealDataService));

            // Close the dialog
            CloseOnConfirmAction?.Invoke();
        }


    }
}
