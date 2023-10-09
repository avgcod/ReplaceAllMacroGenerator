using Avalonia.Controls;
using System.Collections.Generic;
using ReplaceAllMacroGenerator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using ReplaceAllMacroGenerator.Services;
using Avalonia.Platform.Storage;
using System.Collections.ObjectModel;
using ReplaceAllMacroGenerator.Views;
using System;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Collections;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IRecipient<POMessage>
    {
        private readonly Window _parentWindow;
        private readonly IMessenger _messenger; 

        [ObservableProperty]
        private bool _isCalcMacro = true;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private bool _isGenerating = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        public ObservableCollection<POInfo> _poInformation = new ObservableCollection<POInfo>();

        public bool CanAdd => !IsGenerating && !IsAdding;
        public bool CanGenerate => PoInformation.Any()
                && !IsGenerating && !IsAdding;              

        public MainWindowViewModel(Window parentWindow, IMessenger messenger)
        {
            _parentWindow = parentWindow;

            _messenger = messenger;
            _messenger.Register<POMessage>(this);

            _parentWindow.Closing += WindowClosing;
        }

        private void WindowClosing(object? sender, WindowClosingEventArgs e)
        {
            _messenger.Unregister<POMessage>(this);
            _parentWindow.Closing -= WindowClosing;
;        }

        #region Commands
        [RelayCommand]
        public void MacroTypeCheckedChanged(object? parameter)
        {
            string macroType = parameter?.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(macroType) && macroType == "Calc")
            {
                IsCalcMacro = true;
            }
            else if (!string.IsNullOrWhiteSpace(macroType) && macroType != "Calc")
            {
                IsCalcMacro = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanAdd))]
        public async Task Load()
        {
            IsAdding = true;

            IStorageFile? selectedFile = await FileAccessService.ChooseCSVFileAsync(_parentWindow);
            if (selectedFile != null)
            {
                string? fileName = await selectedFile?.SaveBookmarkAsync();
                if (!string.IsNullOrEmpty(fileName))
                {
                    AddInformation(await FileAccessService.LoadCSVAsync(fileName));
                }
            }

            IsAdding = false;
        }

        [RelayCommand(CanExecute = nameof(CanAdd))]
        public async Task Add()
        {
            IsAdding = true;

            AddPOView addView = new AddPOView();
            addView.DataContext = new AddPOViewModel(addView, _messenger);
            await addView.ShowDialog(_parentWindow);

            IsAdding = false;
        }

        [RelayCommand(CanExecute = nameof(CanGenerate))]
        public async Task Generate()
        {
            IsGenerating = true;

            IEnumerable<string> generatedMacro;

            IStorageFile? selectedFile = await FileAccessService.ChooseBASFileAsync(_parentWindow);

            if (selectedFile != null)
            {
                string? fileName = await selectedFile?.SaveBookmarkAsync();

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    if (IsCalcMacro)
                    {
                        generatedMacro = GenerateCalcMacro();
                    }
                    else
                    {
                        generatedMacro = GenerateExcelMacro();
                    }

                    await FileAccessService.SaveMacroFileAsync(generatedMacro, fileName);
                }

                MessageBoxView mboxView = new MessageBoxView();
                mboxView.DataContext = new MessageBoxViewModel(mboxView, "Macro Generated");
                await mboxView.ShowDialog(_parentWindow);
            }

            IsGenerating = false;
        } 
        #endregion

        public void AddInformation(List<POInfo> theInformation)
        {
            PoInformation = new ObservableCollection<POInfo>(theInformation);
        }

        public void AddInformation(POInfo theInformation)
        {
            PoInformation.Add(theInformation);
        }

        private IEnumerable<string> GenerateExcelMacro()
        {
            string firstLine = "Sub FindReplaceAll()" + Environment.NewLine + "'PURPOSE: Find & Replace text/values throughout a specific sheet" + Environment.NewLine + "'SOURCE: www.TheSpreadsheetGuru.com" + Environment.NewLine + Environment.NewLine + "Dim sht As Worksheet" + Environment.NewLine + "Dim fnd As Variant" + Environment.NewLine + "Dim rplc As Variant" + Environment.NewLine + Environment.NewLine + "'Store a specfic sheet to a variable" + Environment.NewLine + "Set sht = Sheets(\"Sheet1\")";
            string lastLine = Environment.NewLine + "End Sub";
            List<string> infoList = new List<string>();
            infoList.Add(firstLine);
            foreach (POInfo currentPOInfo in PoInformation)
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
            infoList.Add(firstLine);
            foreach (POInfo currentPOInfo in PoInformation)
            {
                infoList.Add("fnd = \"" + currentPOInfo.OldPO + "\"" + Environment.NewLine + "rplc = \"" + currentPOInfo.NewPO + "\"" + Environment.NewLine + "oDesc.SearchString=fnd" + Environment.NewLine + "oDesc.ReplaceString=rplc" + Environment.NewLine + "oDoc.replaceAll(oDesc)" + Environment.NewLine);
            }
            infoList.Add(lastLine);
            return infoList;
        }

        public void Receive(POMessage message)
        {
            AddInformation(message.theInfo);
        }
    }
}
