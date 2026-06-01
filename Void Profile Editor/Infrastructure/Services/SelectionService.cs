using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Domain.Services;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Adapters;


namespace Void_Profile_Editor.Infrastructure.Services
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

        public CSharpFunctionalExtensions.Result<Point3DDomain> PickPoint()
        {
            try
            {
                Reference reference = _commandData.Application.ActiveUIDocument.Selection.PickObject(ObjectType.PointOnElement, "Выберите точку");
                Point3DDomain point = reference.GlobalPoint.ToDomain();
                return CSharpFunctionalExtensions.Result.Success(point);
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return CSharpFunctionalExtensions.Result.Failure<Point3DDomain>("Выбор точки отменен пользователем");
            }
        }
    }
}
