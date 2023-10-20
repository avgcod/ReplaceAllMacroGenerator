using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public partial class MessageBoxViewModel : ViewModelBase
    {
        private readonly Window _currentWindow;

        [ObservableProperty]
        private string _messageText = string.Empty;


        public MessageBoxViewModel(Window currentWindow, string messageText)
        {
            MessageText = messageText;
            _currentWindow = currentWindow;
        }

        /// <summary>
        /// Command to handle the OK button being pressed.
        /// </summary>
        [RelayCommand]
        public void OK()
        {
            _currentWindow.Close();
        }
    }
}
