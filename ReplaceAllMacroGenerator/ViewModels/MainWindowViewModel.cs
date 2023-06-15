using Avalonia.Controls;
using ReactiveUI;
using System.Windows.Input;
using ReplaceAllMacroGenerator.Commands;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DynamicData;
using ReplaceAllMacroGenerator.Helpers;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isCalcMacro = true;
        public bool IsCalcMacro
        {
            get
            {
                return _isCalcMacro;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _isCalcMacro, value);
            }
        }

        private bool _isGenerating = false;
        public bool IsGenerating
        {
            get
            {
                return _isGenerating;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _isGenerating, value);
            }
        }

        private List<POInfo> _poInformation = new List<POInfo>();
        public IEnumerable<POInfo> POInformation => _poInformation;

        #region Variables
        private readonly Window _parentWindow;
        #endregion

        #region Commands
        public ICommand AddCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand GenerateCommand { get; }
        public ICommand MacroTypeCheckedChangedCommand { get; }
        #endregion

        public MainWindowViewModel(Window parentWindow)
        {
            _parentWindow = parentWindow;

            AddCommand = new AddCommand(_parentWindow, this);
            LoadCommand = new LoadCommand(parentWindow, this);
            GenerateCommand = new GenerateCommand(parentWindow, this);
            MacroTypeCheckedChangedCommand = new MacroTypeCheckedChangedCommand(parentWindow, this);

        }

        public void AddPOInformation(IEnumerable<POInfo> theInformation)
        {
            _poInformation.Clear();
            _poInformation.AddRange(theInformation);
            this.RaisePropertyChanged(nameof(POInformation));
        }

        public void AddPOInformation(POInfo theInformation)
        {
            _poInformation.Add(theInformation);
            this.RaisePropertyChanged(nameof(POInformation));
        }
    }
}
