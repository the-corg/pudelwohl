﻿using System.Windows;
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

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Increase the window border thickness for the maximized state,
                // otherwise the window would extend beyond the screen edges
                BorderThickness = new Thickness(8);
                MaximizeButton.Visibility = Visibility.Collapsed;
                RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        private void CommandBinding_MinimizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CommandBinding_MaximizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void CommandBinding_RestoreExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void CommandBinding_CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}