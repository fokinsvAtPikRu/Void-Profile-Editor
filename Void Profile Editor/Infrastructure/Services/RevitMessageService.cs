using Autodesk.Revit.UI;
using Void_Profile_Editor.Infrastructure.Abstraction;

namespace Void_Profile_Editor.Infrastructure.Services
{
    public class RevitMessageService : IRevitMessageService
    {
        public void ShowMessage(string head, string message)
        {
            TaskDialog.Show(head, message);
        }
    }
}
