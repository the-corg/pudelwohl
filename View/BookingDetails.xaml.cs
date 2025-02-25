using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class BookingDetails : Window
    {
        private readonly BookingDetailsViewModel _viewModel;
        public BookingDetails(string headerText, GuestViewModel guest, Booking? booking = null)
        {
            Owner = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            InitializeComponent();
            _viewModel = new BookingDetailsViewModel(this, headerText, guest, booking);
            DataContext = _viewModel;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

