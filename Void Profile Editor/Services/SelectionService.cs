using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Void_Profile_Editor.Services
{
    public class SelectionService
    {
        ExternalCommandData _commandData;
        public SelectionService(ExternalCommandData commandData)
        {
            _commandData = commandData;
        }

        public FamilyInstance PickObject()
        {
            Reference reference = _commandData.Application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, new FamilyInstanceSelectionFilter(), "Выберите элемент");
            FamilyInstance element = _commandData.Application.ActiveUIDocument.Document.GetElement(reference) as FamilyInstance;
            return element;
        }
    }
}
