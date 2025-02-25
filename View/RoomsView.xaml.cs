using System.Windows.Controls;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class RoomsView : UserControl
    {
        private readonly RoomsViewModel _viewModel;

        public RoomsView()
        {
            InitializeComponent();
            _viewModel = new RoomsViewModel(new RoomDataProvider());
            DataContext = _viewModel;
            Loaded += RoomsView_LoadedAsync;
        }

        private async void RoomsView_LoadedAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }
    }
}
