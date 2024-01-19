using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReplaceAllMacroGenerator.ViewModels;
using System;

namespace ReplaceAllMacroGenerator
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            string? name = data?.GetType().FullName!.Replace("ViewModel", "View");
            if (name != null)
            {
                var type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type)!;
                }
                else
                {
                    return new TextBlock { Text = "Not Found: " + name };
                }
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}