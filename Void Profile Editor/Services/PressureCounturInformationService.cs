using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Services
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

        public CSharpFunctionalExtensions.Result<PressureContour> CreatePressureContourInfo(FamilyInstance instance)
        {
            if (instance == null)
                return CSharpFunctionalExtensions.Result.Failure<PressureContour>("Instance is null");
            try
            {
                PressureContour contour = new PressureContour()
                {
                    Id = instance.Id,
                    InsertPoint = ((LocationPoint)instance.Location).Point,
                    Rotation = ((LocationPoint)instance.Location).Rotation,
                    ContourParameters = new PressureContourParameters()
                };
                PressureContourParameters parameters = new PressureContourParameters();
                foreach (var key in contour.ContourParameters.DoubleParameters.Keys)
                {
                    parameters.DoubleParameters[key] = instance.LookupParameter(key).AsDouble();
                }
                foreach (var key in contour.ContourParameters.IntParameters.Keys)
                {
                    parameters.IntParameters[key] = instance.LookupParameter(key).AsInteger();
                }
                contour.ContourParameters = parameters;
                return contour;
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure<PressureContour>(ex.Message);
            }
        }

        public CSharpFunctionalExtensions.Result UpdateParameters(Document doc, FamilyInstance instance, PressureContourParameters parameters)
        {
            if (doc == null)
                return CSharpFunctionalExtensions.Result.Failure("document == null");
            if (parameters == null)
                return CSharpFunctionalExtensions.Result.Failure("parameters == null");
            using (Transaction trans = new Transaction(doc, "Изменение параметров контура продавливания"))
            {
                trans.Start();
                Parameter parameter;
                foreach (var key in parameters.DoubleParameters.Keys)
                {
                    parameter = instance.LookupParameter(key);
                    if (!parameter.IsReadOnly)
                        parameter.Set(parameters.DoubleParameters[key]);
                }
                foreach (var key in parameters.IntParameters.Keys)
                {
                    parameter = instance.LookupParameter(key);
                    if (!parameter.IsReadOnly && !String.IsNullOrEmpty(key))
                        parameter.Set(parameters.IntParameters[key]);
                }
                trans.Commit();
            }
            return CSharpFunctionalExtensions.Result.Success();
        }

        public XYZ GetCenterPressureContur(PressureContour contour)
        {
            XYZ center = new XYZ(
                contour.InsertPoint.X,
                contour.InsertPoint.Y + contour.ContourParameters.DoubleParameters["Толщина"] + contour.ContourParameters.DoubleParameters["h0"],
                0);
            return _geometryService.RotatePointAroundAxis(
                center,
                contour.InsertPoint,
                XYZ.BasisZ,
                contour.Rotation);
        }
    }
}
