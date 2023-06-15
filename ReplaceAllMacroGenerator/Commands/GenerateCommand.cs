using Avalonia.Controls;
using ReplaceAllMacroGenerator.ViewModels;
using ReplaceAllMacroGenerator.Views;
using ReplaceAllMacroGenerator.Helpers;
using ReplaceAllMacroGenerator.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceAllMacroGenerator.Commands
{
    public class GenerateCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Window _currentWindow;

        public override bool CanExecute(object? parameter)
        {
            return _mainWindowViewModel.POInformation.Any()
                && !_mainWindowViewModel.IsGenerating
                && base.CanExecute(parameter);
        }
        public async override void Execute(object? parameter)
        {
            _mainWindowViewModel.IsGenerating = true;

            IEnumerable<string> generatedMacro;
            SaveFileDialog sfd = new SaveFileDialog();

            FileDialogFilter filters = new FileDialogFilter();
            filters.Name = "BAS Files (.bas)";
            filters.Extensions.Add("bas");

            sfd.Filters = new List<FileDialogFilter>();
            sfd.Filters.Add(filters);

            string theFile = await sfd.ShowAsync(_currentWindow) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(theFile))
            {
                if (_mainWindowViewModel.IsCalcMacro)
                {
                    generatedMacro = GenerateCalcMacro();
                }
                else
                {
                    generatedMacro = GenerateExcelMacro();
                }
                FileAccessService fileService = new FileAccessService();
                await fileService.SaveMacroFileAsync(generatedMacro, theFile);
            }

            MessageBoxView mboxView = new MessageBoxView();
            mboxView.DataContext = new MessageBoxViewModel(mboxView, "Macro Generated");
            await mboxView.ShowDialog(_currentWindow);

            _mainWindowViewModel.IsGenerating = false;

        }

        public GenerateCommand(Window currentWindow, MainWindowViewModel mainWindowViewModel)
        {
            _currentWindow = currentWindow;
            _mainWindowViewModel = mainWindowViewModel;

            _mainWindowViewModel.PropertyChanged += PropertChanged;
        }

        private void PropertChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.POInformation))
            {
                OnCanExecutedChanged();
            }
        }

        private IEnumerable<string> GenerateExcelMacro()
        {
            string firstLine = "Sub FindReplaceAll()" + Environment.NewLine + "'PURPOSE: Find & Replace text/values throughout a specific sheet" + Environment.NewLine + "'SOURCE: www.TheSpreadsheetGuru.com" + Environment.NewLine + Environment.NewLine + "Dim sht As Worksheet" + Environment.NewLine + "Dim fnd As Variant" + Environment.NewLine + "Dim rplc As Variant" + Environment.NewLine + Environment.NewLine + "'Store a specfic sheet to a variable" + Environment.NewLine + "Set sht = Sheets(\"Sheet1\")";
            string lastLine = Environment.NewLine + "End Sub";
            List<string> infoList = new List<string>();
            string empty = string.Empty;
            infoList.Add(firstLine);
            foreach (POInfo currentPOInfo in _mainWindowViewModel.POInformation)
            {
                infoList.Add("fnd = \"" + currentPOInfo.OldPO + "\"" + Environment.NewLine + "rplc = \"" + currentPOInfo.NewPO + "\"" + Environment.NewLine + "sht.Cells.Replace what:= fnd, Replacement:= rplc, _" + Environment.NewLine + "LookAt:= xlPart, SearchOrder:= xlByRows, MatchCase:= False, _" + Environment.NewLine + "SearchFormat:= False, ReplaceFormat:= False" + Environment.NewLine);
            }
            infoList.Add(lastLine);
            return infoList;
        }

        private IEnumerable<string> GenerateCalcMacro()
        {
            string firstLine = "REM  *****  BASIC  *****" + Environment.NewLine + "Sub FindReplaceAll()" + Environment.NewLine + "'PURPOSE: Find & Replace text/values" + Environment.NewLine + "'SOURCE: https://ask.libreoffice.org/t/find-and-replace-macro/27562 JohnSUN" + Environment.NewLine + "Dim oDoc as object" + Environment.NewLine + "Dim oDesc as object" + Environment.NewLine + "oDoc=ThisComponent.CurrentController.getActiveSheet()" + Environment.NewLine + "oDesc= oDoc.createReplaceDescriptor()" + Environment.NewLine + "oDesc.SearchCaseSensitive=false 'case insensitive" + Environment.NewLine + "oDesc.SearchRegularExpression=false 'no regexp" + Environment.NewLine + Environment.NewLine;
            string lastLine = Environment.NewLine + "End Sub";
            List<string> infoList = new List<string>();
            string empty = string.Empty;
            infoList.Add(firstLine);
            foreach (POInfo currentPOInfo in _mainWindowViewModel.POInformation)
            {
                infoList.Add("fnd = \"" + currentPOInfo.OldPO + "\"" + Environment.NewLine + "rplc = \"" + currentPOInfo.NewPO + "\"" + Environment.NewLine + "oDesc.SearchString=fnd" + Environment.NewLine + "oDesc.ReplaceString=rplc" + Environment.NewLine + "oDoc.replaceAll(oDesc)" + Environment.NewLine);
            }
            infoList.Add(lastLine);
            return infoList;
        }
    }
}
