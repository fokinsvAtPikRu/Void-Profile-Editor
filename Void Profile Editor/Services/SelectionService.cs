using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Void_Profile_Editor.Abstraction.Services;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Services
{
    public class SelectionService : ISelectionService
    {
        ExternalCommandData _commandData;
        public SelectionService(ExternalCommandData commandData)
        {
            _commandData = commandData;
        }

        public CSharpFunctionalExtensions.Result<FamilyInstance> PickObject()
        {
            try
            {
                Reference reference = _commandData.Application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, new PushingEndWallSelectionFilter(), "Выберите элемент");
                FamilyInstance element = _commandData.Application.ActiveUIDocument.Document.GetElement(reference) as FamilyInstance;
                return element;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return CSharpFunctionalExtensions.Result.Failure<FamilyInstance>("Выбор объекта отменен пользователем");
            }
        }

        public CSharpFunctionalExtensions.Result<XYZ> PickPoint()
        {
            try
            {
                Reference reference = _commandData.Application.ActiveUIDocument.Selection.PickObject(ObjectType.PointOnElement, "Выберите точку");
                XYZ point = reference.GlobalPoint;
                return point;
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return CSharpFunctionalExtensions.Result.Failure<XYZ>("Выбор точки отменен пользователем");
            }
        }
    }
}
