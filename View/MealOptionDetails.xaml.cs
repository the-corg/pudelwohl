using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View
{
    /// <summary>
    /// Interaction logic for MealOptionDetails.xaml
    /// </summary>
    public partial class MealOptionDetails : Window
    {
        public MealOptionDetails()
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
