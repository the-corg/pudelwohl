using System.Windows;
using System.Windows.Input;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff
{
    /// <summary>
    /// MVVM-friendly code-behind for MainWindow.xaml.
    /// For the most part, it's strictly view-related code.
    /// Additionally, two DataContext methods are called, to handle 
    /// startup (initialize data asynchronously after the UI is loaded)
    /// and exit (to save data asynchronously on exit) to avoid creating 
    /// extra complexity of attached behaviors for silly reasons.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private fields and the constructor

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
        #endregion

        #region Handling startup and closing

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
        #endregion

        #region Handling window state changes

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
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
        #endregion
    }
}