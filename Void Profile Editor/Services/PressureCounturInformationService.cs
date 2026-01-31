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
        private GeometryService _geometryService;
        public PressureCounturInformationService(
            Document document,
            GeometryService geometryService)
        {
            _document = document;
            _geometryService = geometryService;
        }

        public CSharpFunctionalExtensions.Result<PressureContour> CreatePressureContourInfo(FamilyInstance instance)
        {
            try
            {
                PressureContour contour = new PressureContour()
                {
                    Id = instance.Id,
                    H0 = instance.LookupParameter("h0").AsDouble(),
                    WallThickness = instance.LookupParameter("Толщина").AsDouble(),
                    InsertPoint = ((LocationPoint)instance.Location).Point,
                    Rotation = ((LocationPoint)instance.Location).Rotation
                };
                return contour;
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure<PressureContour>(ex.Message);
            }
        }
        
        public XYZ GetCenterPressureContur(PressureContour contour)
        {
            XYZ center = new XYZ(
                contour.InsertPoint.X,
                contour.InsertPoint.Y + contour.WallThickness + contour.H0 / 2,
                0);
            return _geometryService.RotatePointAroundAxis(
                center,
                contour.InsertPoint,
                XYZ.BasisZ, 
                contour.Rotation);
        }
    }
}
