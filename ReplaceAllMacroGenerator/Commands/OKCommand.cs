using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceAllMacroGenerator.Commands
{
    public class OKCommand : CommandBase
    {
        private readonly Window _currentWindow;

        public OKCommand(Window currentWindow)
        {
            _currentWindow = currentWindow;
        }

        public override void Execute(object? parameter)
        {
            _currentWindow.Close();
        }
    }
}
