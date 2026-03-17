using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace MyMenu
{
    [Transaction(TransactionMode.Manual)]
    public class RevitMenu : IExternalApplication
    {

        public Result OnStartup(UIControlledApplication application)
        {
            // получаем путь к AppData
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            application.CreateRibbonTab("Автоматизация Revit");
            var panel = application.CreateRibbonPanel("Автоматизация Revit", "Общее");
            string dllPath = Path.Combine(appDataPath,
                @"Autodesk\Revit\Addins\2019\VoidProfileEditor\Void Profile Editor.dll");
            var button = new PushButtonData(
                "myButton",
                "VoidProfileEditor",
                dllPath,
                "Void_Profile_Editor.Cmd"
                );
            string imagePath = Path.Combine(appDataPath,
                @"Autodesk\Revit\Addins\2019\MyMenu\img\Validate32.png");
            BitmapImage bitmap = new BitmapImage(
                new Uri(imagePath, UriKind.Absolute));
            button.LargeImage = bitmap;
            panel.AddItem(button);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
