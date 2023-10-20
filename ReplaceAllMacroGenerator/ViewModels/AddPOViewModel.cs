using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ReplaceAllMacroGenerator.Models;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public partial class AddPOViewModel : ViewModelBase
    {
        private readonly Window _currentWindow;
        private readonly IMessenger _messenger;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OKCommand))]
        private string _oldPO = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OKCommand))]
        private string _newPO = string.Empty;
             
        public AddPOViewModel(Window currentWindow, IMessenger messenger)
        {
            _currentWindow = currentWindow;
            _messenger = messenger;
        }

        /// <summary>
        /// CanExecute for the Add Command.
        /// </summary>
        public bool CanAdd => !string.IsNullOrEmpty(OldPO) && !string.IsNullOrEmpty(NewPO);

        /// <summary>
        /// Command to handle the OK button being pressed.
        /// </summary>
        [RelayCommand(CanExecute =nameof(CanAdd))]
        public void OK()
        {
            POInfo theInfo = new POInfo()
            {
                OldPO = OldPO,
                NewPO = NewPO
            };
            _messenger.Send<POMessage>(new POMessage(theInfo));
            _currentWindow.Close();
        }

        /// <summary>
        /// Command to handle the Cancel button being pressed.
        /// </summary>
        [RelayCommand]
        public void Cancel()
        {
            _currentWindow.Close();
        }

    }
}
