using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Microsoft.Extensions.DependencyInjection;
using RxBim.Command.Revit;
using RxBim.Shared;
using System;
using Void_Profile_Editor.Views;

namespace Void_Profile_Editor
{
    [Transaction(TransactionMode.Manual)]
    public class Cmd : RxBimCommand
    {
        public PluginResult ExecuteCommand(IServiceProvider provider)
        {
            var mainWindow = provider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            return PluginResult.Succeeded;
        }
    }
}
