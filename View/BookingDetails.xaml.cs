using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataServices;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class BookingDetails : Window
    {
        private readonly BookingDetailsViewModel _viewModel;
        public BookingDetails(IRoomDataService roomDataService, string headerText, int guestId, Booking? booking = null)
        {
            Owner = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            InitializeComponent();
            _viewModel = new BookingDetailsViewModel(roomDataService, this, headerText, guestId, booking);
            DataContext = _viewModel;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

