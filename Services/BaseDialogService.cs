using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    /// <summary>
    /// Base class for dialog services,
    /// stores a reference to MainWindow,
    /// provides a ShowDialog method implementation
    /// </summary>
    public abstract class BaseDialogService
    {
        private Window? _mainWindow;
        private Window? MainWindow => _mainWindow ??= Application.Current.MainWindow; // Lazy initialization

        /// <summary>
        /// Dims the main window and shows <paramref name="dialog"/>.<br/>
        /// Afterwards restores the main window's opacity
        /// and returns the result of type bool that shows
        /// whether the activity was confirmed or canceled.
        /// </summary>
        /// <param name="dialog">The dialog window to show</param>
        /// <returns>True, if the activity was confirmed<br/>
        /// False, if the activity was canceled</returns>
        protected bool ShowDialog(Window dialog)
        {
            // Dim main window before showing the modal window, then restore it back
            if (MainWindow is not null) MainWindow.Opacity = 0.4;
            bool? result = dialog.ShowDialog();
            if (MainWindow is not null) MainWindow.Opacity = 1.0;
            return result == true;
        }
    }
}