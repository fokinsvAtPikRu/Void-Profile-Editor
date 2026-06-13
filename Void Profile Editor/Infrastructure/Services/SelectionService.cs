using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Void_Profile_Editor.Domain.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Adapters;


namespace Void_Profile_Editor.Infrastructure.Services
{
    public class SelectionService : ISelectionService
    {
        ExternalCommandData _commandData;
        IAllowedFamiliesConfig _jsonConfig;
        public SelectionService(
            ExternalCommandData commandData,
            IAllowedFamiliesConfig jsonConfig)
        {
            _commandData = commandData;
            _jsonConfig = jsonConfig;
        }

        public CSharpFunctionalExtensions.Result<FamilyInstance> PickObject()
        {
            try
            {

                Reference reference = _commandData.Application.ActiveUIDocument.Selection.PickObject(
                    ObjectType.Element, 
                    new CalculatedPushingSelectionFilter(_jsonConfig), 
                    "Выберите элемент");
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
