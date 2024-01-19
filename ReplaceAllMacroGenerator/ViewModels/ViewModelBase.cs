using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ReplaceAllMacroGenerator.ViewModels
{
    public class ViewModelBase(IMessenger theMessenger) : ObservableRecipient(theMessenger);
}
