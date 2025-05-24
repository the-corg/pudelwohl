using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    public abstract class BaseDialogService
    {
        private Window? _mainWindow;
        private Window? MainWindow => _mainWindow ??= Application.Current.MainWindow; // Lazy initialization

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