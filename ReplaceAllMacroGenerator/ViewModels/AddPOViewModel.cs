using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public partial class AddPOViewModel : ViewModelBase
    {
        private readonly Window _currentWindow;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OKCommand))]
        private string _oldPO = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OKCommand))]
        private string _newPO = string.Empty;

        public bool OKResult { get; set; } = false;

        public bool CanAdd => !string.IsNullOrEmpty(OldPO) && !string.IsNullOrEmpty(NewPO);

        public AddPOViewModel(Window currentWindow)
        {
            _currentWindow = currentWindow;
        }

        [RelayCommand(CanExecute =nameof(CanAdd))]
        public void OK()
        {
            OKResult = true;
            _currentWindow.Close();
        }

        [RelayCommand]
        public void Cancel()
        {
            NewPO = string.Empty;
            _currentWindow.Close();
        }

    }
}
