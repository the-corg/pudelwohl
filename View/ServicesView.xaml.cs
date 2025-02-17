using System.Windows.Controls;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class ServicesView : UserControl
    {
        private readonly ServicesViewModel _viewModel;

        public ServicesView()
        {
            InitializeComponent();
            _viewModel = new ServicesViewModel(new ServiceDataProvider());
            DataContext = _viewModel;
            Loaded += ServicesView_LoadedAsync;
        }

        private async void ServicesView_LoadedAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }
    }
}
