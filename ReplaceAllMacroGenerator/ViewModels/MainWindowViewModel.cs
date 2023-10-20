using Avalonia.Controls;
using System.Collections.Generic;
using ReplaceAllMacroGenerator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using ReplaceAllMacroGenerator.Services;
using System.Collections.ObjectModel;
using ReplaceAllMacroGenerator.Views;
using System;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IRecipient<POMessage>, IRecipient<OperationErrorMessage>
    {
        #region Variables
        /// <summary>
        /// Program window.
        /// </summary>
        private readonly Window _parentWindow;
        /// <summary>
        /// CommunityToolkit MVVM messenger.
        /// </summary>
        private readonly IMessenger _messenger;
        /// <summary>
        /// File extension for files to load.
        /// </summary>
        private readonly string _openExtension;
        /// <summary>
        /// File extension for the saved file.
        /// </summary>
        private readonly string _saveExtension;
        #endregion

        #region Properties
        /// <summary>
        /// If the macro being generated is for LibreOffice Calc. If not, the macro will be for Microsoft Excel.
        /// </summary>
        [ObservableProperty]
        private bool _isCalcMacro = true;

        /// <summary>
        /// If the program is busy.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private bool _busy = false;

        /// <summary>
        /// Collection of POInfo.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        public ObservableCollection<POInfo> _poInformation = new ObservableCollection<POInfo>();
        #endregion

        public MainWindowViewModel(Window parentWindow, IMessenger messenger, string openExtension, string saveExtension)
        {
            _parentWindow = parentWindow;
            _openExtension = openExtension;
            _saveExtension = saveExtension;
            _messenger = messenger;

            _messenger.Register<POMessage>(this);
            _messenger.Register<OperationErrorMessage>(this);

            _parentWindow.Closing += WindowClosing;

        }

        /// <summary>
        /// Handles cleanup when the parent window is being closed.
        /// </summary>
        /// <param name="sender">Window being closed.</param>
        /// <param name="e">Window Closing Event Args</param>
        private void WindowClosing(object? sender, WindowClosingEventArgs e)
        {
            _messenger.UnregisterAll(this);
            _parentWindow.Closing -= WindowClosing;
        }

        #region Commands
        /// <summary>
        /// CanExecute for the Add Command.
        /// </summary>
        public bool CanAdd => !Busy;
        /// <summary>
        /// CanExecute for the Load Command.
        /// </summary>
        public bool CanLoad => !Busy;
        /// <summary>
        /// CanExecute for the Generate Command.
        /// </summary>
        public bool CanGenerate => PoInformation.Any()
                && !Busy;

        /// <summary>
        /// Command to handle the check box state changing.
        /// </summary>
        /// <param name="parameter">Checkbox parameter.</param>
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

        /// <summary>
        /// Command to handle the load button being pressed.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanLoad))]
        public async Task Load()
        {
            Busy = true;

            await LoadPOInformation();

            Busy = false;
        }

        /// <summary>
        /// Command to handle the Add buttong being pressed.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanAdd))]
        public async Task Add()
        {
            Busy = true;

            AddPOView addView = new AddPOView();
            addView.DataContext = new AddPOViewModel(addView, _messenger);
            await addView.ShowDialog(_parentWindow);

            Busy = false;
        }

        /// <summary>
        /// Command to handle when the Generate button being pressed.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanGenerate))]
        public async Task Generate()
        {
            Busy = true;

            await SaveMacroFile();

            Busy = false;
        }

        /// <summary>
        /// Saves the macro file.
        /// </summary>
        /// <returns>Task</returns>
        private async Task SaveMacroFile()
        {
            IEnumerable<string> generatedMacro;

            string selectedFile = await FileAccessService.ChooseSaveFileAsync(_parentWindow, _saveExtension, _messenger);

            if (!string.IsNullOrEmpty(selectedFile))
            {
                if (IsCalcMacro)
                {
                    generatedMacro = await GenerateFindReplaceAllCalcMacroAsync();
                }
                else
                {
                    generatedMacro = await GenerateFindReplaceAllExcelMacroAsync();
                }

                await FileAccessService.SaveMacroFileAsync(generatedMacro, selectedFile, _messenger);
                await ShowMessageBox("Macro Generated");
            }
        }

        
        #endregion

        /// <summary>
        /// Loads PoInformation from a file.
        /// </summary>
        public async Task LoadPOInformation()
        {
            string selectedFile = await FileAccessService.ChooseOpenFileAsync(_parentWindow, _openExtension, _messenger);
            if (!string.IsNullOrEmpty(selectedFile))
            {
                PoInformation = new ObservableCollection<POInfo>(await FileAccessService.LoadCSVAsync(selectedFile, _messenger));
            }
        }

        /// <summary>
        /// Generates a find replace all microsoft Excel macro.
        /// </summary>
        /// <returns>Collections of strings representing the macro.</returns>
        private async Task<IEnumerable<string>> GenerateFindReplaceAllExcelMacroAsync()
        {
            string firstLine = "Sub FindReplaceAll()" + Environment.NewLine + "'PURPOSE: Find & Replace text/values throughout a specific sheet" + Environment.NewLine + "'SOURCE: www.TheSpreadsheetGuru.com" + Environment.NewLine + Environment.NewLine + "Dim sht As Worksheet" + Environment.NewLine + "Dim fnd As Variant" + Environment.NewLine + "Dim rplc As Variant" + Environment.NewLine + Environment.NewLine + "'Store a specfic sheet to a variable" + Environment.NewLine + "Set sht = Sheets(\"Sheet1\")";
            string lastLine = Environment.NewLine + "End Sub";
            List<string> infoList = new List<string>();
            infoList.Add(firstLine);
            await Task.Run(() =>
            {
                foreach (POInfo currentPOInfo in PoInformation)
                {
                    infoList.Add("fnd = \"" + currentPOInfo.OldPO + "\"" + Environment.NewLine + "rplc = \"" + currentPOInfo.NewPO + "\"" + Environment.NewLine + "sht.Cells.Replace what:= fnd, Replacement:= rplc, _" + Environment.NewLine + "LookAt:= xlPart, SearchOrder:= xlByRows, MatchCase:= False, _" + Environment.NewLine + "SearchFormat:= False, ReplaceFormat:= False" + Environment.NewLine);
                }
            });

            infoList.Add(lastLine);
            return infoList;
        }

        /// <summary>
        /// Generates a find replace all LibreOffice Calc macro.
        /// </summary>
        /// <returns>Collections of strings representing the macro.</returns>
        private async Task<IEnumerable<string>> GenerateFindReplaceAllCalcMacroAsync()
        {
            string firstLine = "REM  *****  BASIC  *****" + Environment.NewLine + "Sub FindReplaceAll()" + Environment.NewLine + "'PURPOSE: Find & Replace text/values" + Environment.NewLine + "'SOURCE: https://ask.libreoffice.org/t/find-and-replace-macro/27562 JohnSUN" + Environment.NewLine + "Dim oDoc as object" + Environment.NewLine + "Dim oDesc as object" + Environment.NewLine + "oDoc=ThisComponent.CurrentController.getActiveSheet()" + Environment.NewLine + "oDesc= oDoc.createReplaceDescriptor()" + Environment.NewLine + "oDesc.SearchCaseSensitive=false 'case insensitive" + Environment.NewLine + "oDesc.SearchRegularExpression=false 'no regexp" + Environment.NewLine + Environment.NewLine;
            string lastLine = Environment.NewLine + "End Sub";
            List<string> infoList = new List<string>();
            infoList.Add(firstLine);
            await Task.Run(() =>
            {
                foreach (POInfo currentPOInfo in PoInformation)
                {
                    infoList.Add("fnd = \"" + currentPOInfo.OldPO + "\"" + Environment.NewLine + "rplc = \"" + currentPOInfo.NewPO + "\"" + Environment.NewLine + "oDesc.SearchString=fnd" + Environment.NewLine + "oDesc.ReplaceString=rplc" + Environment.NewLine + "oDoc.replaceAll(oDesc)" + Environment.NewLine);
                }
            });

            infoList.Add(lastLine);
            return infoList;
        }

        #region Message Handling
        /// <summary>
        /// Handles a received POMessage.
        /// </summary>
        /// <param name="theMessage">The POMessage to handle.</param>
        public void HandlePOMessage(POMessage theMessage)
        {
            PoInformation.Add(theMessage.TheInfo);
        }

        /// <summary>
        /// Handles a received OperationErrorMessage.
        /// </summary>
        /// <param name="theMessage">The OperationErrorMessage to handle.</param>
        public async Task HandleOperationErrorMessage(OperationErrorMessage theMessage)
        {
            ErrorMessageBoxView emboxView = new ErrorMessageBoxView();

            emboxView.DataContext = new ErrorMessageBoxViewModel(emboxView, theMessage);

            await emboxView.ShowDialog(_parentWindow);
        }

        /// <summary>
        /// Received POMessage messages.
        /// </summary>
        /// <param name="message">POMessage message received.</param>
        public void Receive(POMessage message)
        {
            HandlePOMessage(message);
        }

        /// <summary>
        /// Received OperationErrorMessage messages.
        /// </summary>
        /// <param name="message">OperationErrorMessage message received.</param>
        public async void Receive(OperationErrorMessage message)
        {
            await HandleOperationErrorMessage(message);
        }
        #endregion

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <returns>Task</returns>
        private async Task ShowMessageBox(string message)
        {
            MessageBoxView mboxView = new MessageBoxView();
            mboxView.DataContext = new MessageBoxViewModel(mboxView, message);
            await mboxView.ShowDialog(_parentWindow);
        }

    }
}
