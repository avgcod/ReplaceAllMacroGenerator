using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ReplaceAllMacroGenerator.Models;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public partial class AddReplacementInfoViewModel(Window currentWindow, IMessenger messenger) : ViewModelBase(messenger)
    {
        private readonly Window _currentWindow = currentWindow;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OKCommand))]
        private string _oldPO = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OKCommand))]
        private string _newPO = string.Empty;

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
            ReplacementInfo theInfo = new()
            {
                OldInfo = OldPO,
                NewInfo = NewPO
            };
            Messenger.Send<POMessage>(new POMessage(theInfo));
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
