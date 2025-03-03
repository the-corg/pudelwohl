using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    public partial class BookingDetails : Window
    {
        private readonly BookingDetailsViewModel _viewModel;
        public BookingDetails(IGuestDataService guestDataService, IRoomDataService roomDataService, 
            string headerText, bool isGuestSelectable, bool isRoomSelectable, int guestId, int roomId, Booking? booking)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            _viewModel = new BookingDetailsViewModel(guestDataService, roomDataService,
                headerText, isGuestSelectable, isRoomSelectable, guestId, roomId, booking);
            DataContext = _viewModel;

            _viewModel.CloseOnConfirmAction = () => DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // closes the window
        }
    }
}

