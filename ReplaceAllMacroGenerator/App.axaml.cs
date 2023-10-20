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
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.DataContext = new MainWindowViewModel(desktop.MainWindow, StrongReferenceMessenger.Default, ".csv",".bas");
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}