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
    public class MacroTypeCheckedChangedCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Window _currentWindow;

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter);
        }
        public override void Execute(object? parameter)
        {
            string macroType = parameter?.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(macroType) && macroType == "Calc")
            {
                _mainWindowViewModel.IsCalcMacro = true;
            }
            else if (!string.IsNullOrWhiteSpace(macroType) && macroType != "Calc")
            {
                _mainWindowViewModel.IsCalcMacro = false;
            }
        }

        public MacroTypeCheckedChangedCommand(Window currentWindow, MainWindowViewModel mainWindowViewModel)
        {
            _currentWindow = currentWindow;
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}
