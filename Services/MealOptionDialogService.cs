using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    /// <summary>
    /// Manages the Meal option dialog 
    /// </summary>
    public interface IMealOptionDialogService
    {
        /// <summary>
        /// Creates and shows the dialog for adding or editing a meal option.
        /// Also creates and sets up its view model.
        /// </summary>
        /// <param name="headerText">Text for the dialog header</param>
        /// <param name="mealOption">Meal option to be edited (omit for adding a new one)</param>
        /// <returns>True, if the meal option change or adding was confirmed<br/>False, otherwise</returns>
        bool ShowMealOptionDialog(string headerText, MealOptionViewModel? mealOption = null);
    }

    public class MealOptionDialogService : BaseDialogService, IMealOptionDialogService
    {
        #region Private fields and the constructor

        private readonly IMealDataService _mealDataService;

        public MealOptionDialogService(IMealDataService mealDataService)
        {
            _mealDataService = mealDataService;
        }
        #endregion


        #region Public method (see interface)

        public bool ShowMealOptionDialog(string headerText, MealOptionViewModel? mealOption = null)
        {
            var dialog = new MealOptionDetails();
            var viewModel = new MealOptionDetailsViewModel(_mealDataService, headerText, mealOption);
            dialog.DataContext = viewModel;
            viewModel.CloseOnConfirmAction = () => dialog.DialogResult = true;
            return ShowDialog(dialog);
        }
        #endregion
    }
}
