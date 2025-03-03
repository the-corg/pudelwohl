using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services
{
    public interface IMessageService
    {
        void ShowMessage(string message, string caption = "Error");
        bool ShowConfirmation(string message, string caption = "Please Confirm");
    }

    public class MessageService : IMessageService
    {
        public void ShowMessage(string message, string caption = "Error")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowConfirmation(string message, string caption = "Please Confirm")
        {
            return MessageBox.Show(message, caption, MessageBoxButton.YesNo, 
                MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes;
        }
    }
}
