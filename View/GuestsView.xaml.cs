using System.Windows.Controls;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class GuestsView : UserControl
    {
        private readonly GuestsViewModel _viewModel;

        public GuestsView()
        {
            InitializeComponent();
            _viewModel = new GuestsViewModel(new GuestDataProvider());
            DataContext = _viewModel;
            Loaded += GuestsView_LoadedAsync;
        }

        private async void GuestsView_LoadedAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

    }
}
