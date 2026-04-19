using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Services
{
    public class PressureCounturInformationService : IPressureCounturInformationService
    {
        private Autodesk.Revit.DB.Document _document;
        private IGeometryService _geometryService;
        public PressureCounturInformationService(
            Autodesk.Revit.DB.Document document,
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
                    var parametr = instance.LookupParameter(key);
                    if (parametr == null)
                    {
                        throw new Exception($"Параметр {key} не найден");
                    }
                    parameters.DoubleParameters[key] = parametr.AsDouble();
                }
                foreach (var key in contour.ContourParameters.IntParameters.Keys)
                {
                    var parametr = instance.LookupParameter(key);
                    if (parametr == null)
                    {
                        throw new Exception($"Параметр {key} не найден");
                    }
                    parameters.IntParameters[key] = parametr.AsInteger();
                }
                contour.ContourParameters = parameters;
                return contour;
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure<PressureContour>(ex.Message);
            }
        }

        public CSharpFunctionalExtensions.Result UpdateParameters(Autodesk.Revit.DB.Document doc, 
            FamilyInstance instance, 
            PressureContourParameters parameters)
        {
            if (doc == null)
                return CSharpFunctionalExtensions.Result.Failure("document == null");
            if (parameters == null)
                return CSharpFunctionalExtensions.Result.Failure("parameters == null");

            foreach (var key in parameters.DoubleParameters.Keys)
            {
                Autodesk.Revit.DB.Parameter parameter = instance.LookupParameter(key);
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
                Autodesk.Revit.DB.Parameter parameter = instance.LookupParameter(key);
                if (parameter == null)
                {
                    return CSharpFunctionalExtensions.Result.Failure($"Не найден параметр {key} в экземпляре семейства");
                }
                if (parameter.IsReadOnly)
                {
                    return CSharpFunctionalExtensions.Result.Failure($"Параметр {key} доступен только для чтения");
                }
            }

            using (Transaction trans = new Transaction(doc, "Изменение параметров контура продавливания"))
            {
                trans.Start();
                Autodesk.Revit.DB.Parameter parameter;
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
