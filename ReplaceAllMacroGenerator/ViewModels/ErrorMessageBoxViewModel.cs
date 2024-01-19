using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ReplaceAllMacroGenerator.Models;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public partial class ErrorMessageBoxViewModel(Window theWindow, IMessenger theMessenger) : ViewModelBase(theMessenger), IRecipient<OperationErrorMessage>
    {
        private readonly Window _currentWindow = theWindow;

        [ObservableProperty]
        private string _errorType = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [RelayCommand]
        public void OK()
        {
            _currentWindow.Close();
        }

        public void Receive(OperationErrorMessage message)
        {
            ErrorType = message.ErrorType;
            ErrorMessage = message.ErrorMessage;
        }

        protected override void OnActivated()
        {
            Messenger.RegisterAll(this);
            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            Messenger.UnregisterAll(this);
            base.OnDeactivated();
        }
    }
}
