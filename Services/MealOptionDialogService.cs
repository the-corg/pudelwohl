using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    public interface IMealOptionDialogService
    {
        bool ShowMealOptionDialog(string headerText, MealOptionViewModel? mealOption = null);
    }

    public class MealOptionDialogService : BaseDialogService, IMealOptionDialogService
    {
        private readonly IMealDataService _mealDataService;

        public MealOptionDialogService(IMealDataService mealDataService)
        {
            _mealDataService = mealDataService;
        }

        public bool ShowMealOptionDialog(string headerText, MealOptionViewModel? mealOption = null)
        {
            var dialog = new MealOptionDetails();
            var viewModel = new MealOptionDetailsViewModel(_mealDataService, headerText, mealOption);
            dialog.DataContext = viewModel;
            viewModel.CloseOnConfirmAction = () => dialog.DialogResult = true;
            return ShowDialog(dialog);
        }
    }
}
