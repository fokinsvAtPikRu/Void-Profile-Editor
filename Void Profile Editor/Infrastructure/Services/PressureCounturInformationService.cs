using Autodesk.Revit.DB;
using Void_Profile_Editor.Domain.Abstraction.Services;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Adapters;

namespace Void_Profile_Editor.Infrastructure.Services
{
    public class PressureCounturInformationService : IPressureCounturInformationService
    {
        private Document _document;
        private IGeometryService _geometryService;
        public PressureCounturInformationService(
            Document document,
            IGeometryService geometryService)
        {
            _document = document;
            _geometryService = geometryService;
        }

        

        public CSharpFunctionalExtensions.Result UpdateParameters(PressureContour pressureContour)
        {
            
            if (pressureContour == null)
                return CSharpFunctionalExtensions.Result.Failure("PressureContour == null");
            if (pressureContour.ContourParameters == null)
                return CSharpFunctionalExtensions.Result.Failure("PressureContour.Parameters == null");
            var parameters = pressureContour.ContourParameters;
            Element instance = _document.GetElement(pressureContour.Id.ToRevitId());
            if (instance == null)
                return CSharpFunctionalExtensions.Result.Failure("Элемент не найден");
            foreach (var key in parameters.DoubleParameters.Keys)
            {
                Parameter parameter = instance.LookupParameter(key);
                if (parameter == null)
                {
                    return CSharpFunctionalExtensions.Result.Failure($"Не найден параметр {key} в экземпляре семейства");
                }
                if (parameter.IsReadOnly)
                {
                    return CSharpFunctionalExtensions.Result.Failure($"Параметр {key} доступен только для чтения");
                }
            }
            foreach (var key in parameters.IntParameters.Keys)
            {
                Parameter parameter = instance.LookupParameter(key);
                if (parameter == null)
                {
                    return CSharpFunctionalExtensions.Result.Failure($"Не найден параметр {key} в экземпляре семейства");
                }
                if (parameter.IsReadOnly)
                {
                    return CSharpFunctionalExtensions.Result.Failure($"Параметр {key} доступен только для чтения");
                }
            }

            using (Transaction trans = new Transaction(_document, "Изменение параметров контура продавливания"))
            {
                trans.Start();
                Parameter parameter;
                foreach (var key in parameters.DoubleParameters.Keys)
                {
                    parameter = instance.LookupParameter(key);
                    parameter.Set(parameters.DoubleParameters[key]);
                }
                foreach (var key in parameters.IntParameters.Keys)
                {
                    parameter = instance.LookupParameter(key);
                    parameter.Set(parameters.IntParameters[key]);
                }
                trans.Commit();
            }
            return CSharpFunctionalExtensions.Result.Success();
        }

        public Point3DDomain GetCenterPressureContur(PressureContour contour)
        {
            var center = new Point3DDomain(
                contour.InsertPoint.X,
                contour.InsertPoint.Y + contour.ContourParameters.DoubleParameters["Толщина"] + contour.ContourParameters.DoubleParameters["h0"],
                0);
            return _geometryService.RotatePointAroundAxis(
                center,
                contour.InsertPoint,
                XYZ.BasisZ.ToDomain(),
                contour.Rotation);
        }

        
    }
}
