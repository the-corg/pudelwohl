using System.Windows;
using System.Windows.Input;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private bool _readyToBeClosed = false;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_readyToBeClosed) 
                return; // Closes the window

            e.Cancel = true; // Cancel closing for now
            Hide(); // Hide the window immediately

            await _viewModel.SaveDataAsync(); // Save all data

            // Now really close the window
            _readyToBeClosed = true;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Increase the window border thickness for the maximized state,
            // otherwise the window would extend beyond the screen edges for some reason
            BorderThickness = WindowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}