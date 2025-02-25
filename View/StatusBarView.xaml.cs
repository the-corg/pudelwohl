using System.Windows.Controls;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class StatusBarView : UserControl
    {
        private readonly StatusBarViewModel _viewModel;

        public StatusBarView()
        {
            InitializeComponent();
            _viewModel = new StatusBarViewModel();
            DataContext = _viewModel;
        }
    }
}
