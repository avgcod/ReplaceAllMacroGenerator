using Avalonia.Controls;
using ReplaceAllMacroGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceAllMacroGenerator.Commands
{
    public class AddCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Window _currentWindow;

        public override bool CanExecute(object? parameter)
        {
            return !_mainWindowViewModel.IsGenerating
                && base.CanExecute(parameter);
        }
        public override void Execute(object? parameter)
        {

        }

        public AddCommand(Window currentWindow, MainWindowViewModel mainWindowViewModel)
        {
            _currentWindow = currentWindow;
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}
