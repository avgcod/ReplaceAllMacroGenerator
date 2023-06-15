using Avalonia.Controls;
using ReplaceAllMacroGenerator.Helpers;
using Microsoft.VisualBasic;
using ReplaceAllMacroGenerator.Services;
using ReplaceAllMacroGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceAllMacroGenerator.Commands
{
    public class LoadCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Window _currentWindow;

        public override bool CanExecute(object? parameter)
        {
            return !_mainWindowViewModel.IsGenerating
                && base.CanExecute(parameter);
        }
        public async override void Execute(object? parameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AllowMultiple = false;

            FileDialogFilter filters = new FileDialogFilter();
            filters.Name = "CSV Files (.csv)";
            filters.Extensions.Add("csv");

            ofd.Filters = new List<FileDialogFilter>();
            ofd.Filters.Add(filters);

            string[] theFile = await ofd.ShowAsync(_currentWindow) ?? new string[] { string.Empty };
            if (!string.IsNullOrWhiteSpace(theFile[0]))
            {
                FileAccessService fileService = new FileAccessService();
                _mainWindowViewModel.AddPOInformation(fileService.LoadCSV(theFile[0]));
            }
        }

        public LoadCommand(Window currentWindow, MainWindowViewModel mainWindowViewModel)
        {
            _currentWindow = currentWindow;
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}
