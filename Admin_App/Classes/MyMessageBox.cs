using System.Windows;

namespace Admin_App.Classes
{
    public class MyMessageBox
    {
        public void Information(string _messageBoxMessage, string _messageBoxName = "Surveillance Admin", MessageBoxButton _messageBoxButton = MessageBoxButton.OK)
        {
            MessageBox.Show(_messageBoxMessage, _messageBoxName, _messageBoxButton, MessageBoxImage.Information);
        }

        public void Error(string _messageBoxMessage, string _messageBoxName = "Surveillance Admin", MessageBoxButton _messageBoxButton = MessageBoxButton.OK)
        {
            MessageBox.Show(_messageBoxMessage, _messageBoxName, _messageBoxButton, MessageBoxImage.Error);
        }

        public void Exclamation(string _messageBoxMessage, string _messageBoxName = "Surveillance Admin", MessageBoxButton _messageBoxButton = MessageBoxButton.OK)
        {
            MessageBox.Show(_messageBoxMessage, _messageBoxName, _messageBoxButton, MessageBoxImage.Exclamation);
        }
    }
}
