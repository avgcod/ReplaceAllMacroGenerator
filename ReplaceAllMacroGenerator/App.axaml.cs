using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReplaceAllMacroGenerator.ViewModels;
using ReplaceAllMacroGenerator.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace ReplaceAllMacroGenerator
{
    public partial class App : Application
    {
        private MainWindowViewModel? mwvModel;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.Closing += MainWindow_Closing;
                mwvModel = new MainWindowViewModel(desktop.MainWindow,StrongReferenceMessenger.Default, ".csv", ".bas");
                desktop.MainWindow.DataContext = mwvModel;
                mwvModel.IsActive = true;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void MainWindow_Closing(object? sender, Avalonia.Controls.WindowClosingEventArgs e)
        {
            mwvModel!.IsActive = false;
        }
    }
}