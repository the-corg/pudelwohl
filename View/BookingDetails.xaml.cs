using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    /// <summary>
    /// Interaction logic for BookingDetails.xaml
    /// </summary>
    public partial class BookingDetails : Window
    {
        public BookingDetails()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // closes the window
        }
    }
}

