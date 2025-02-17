using System.Windows.Controls;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class MealOptionsView : UserControl
    {
        private readonly MealOptionsViewModel _viewModel;

        public MealOptionsView()
        {
            InitializeComponent();
            _viewModel = new MealOptionsViewModel(new MealOptionDataProvider());
            DataContext = _viewModel;
            Loaded += MealOptionsView_LoadedAsync;
        }

        private async void MealOptionsView_LoadedAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }
    }
}
