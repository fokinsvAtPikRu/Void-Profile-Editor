using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using Void_Profile_Editor.Domain.Abstraction.Configuration;
using Void_Profile_Editor.Domain.Abstraction.Services;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Adapters;

namespace Void_Profile_Editor.Domain.Services
{
    public class CreateContourService :ICreateContourService
    {        
        private IGeometryService _geometryService;
        private IRevitLineService _drawLineService;
        private IAllowedFamiliesConfig _jsonConfig;        

        public CreateContourService(
            IGeometryService geometryService, 
            IRevitLineService drawLineService,
            IAllowedFamiliesConfig jsonConfig)         
        {            
            _geometryService = geometryService;
            _drawLineService=drawLineService;
            _jsonConfig=jsonConfig;
        }
        public CSharpFunctionalExtensions.Result<Contour> Create
            (string familyName,
            Point3DDomain locationPoint,
            double rotationAngle,
            double h0,
            double thickness,
            double offset,
            bool isMirrored)
        {
            try
            {
                // вычисляем координаты углов контура
                var bottomRight = new Point3DDomain
                    (locationPoint.X + thickness / 2 + offset,
                    locationPoint.Y - offset,
                    0);                
                var bottomLeft = new Point3DDomain
                    (locationPoint.X - thickness / 2 - offset,
                    locationPoint.Y - offset,
                    0);
                var topRight = new Point3DDomain
                    (locationPoint.X + thickness / 2 + offset,
                    locationPoint.Y + thickness + 0.5 * h0,
                    0);
                var topLeft = new Point3DDomain
                    (locationPoint.X - thickness / 2 - offset,
                    locationPoint.Y + thickness + 0.5 * h0,
                    0);                
                var center = new Point3DDomain
                    (locationPoint.X,
                    locationPoint.Y + (thickness + 0.5 * h0) * 0.5,
                    0);                    

                rotationAngle = isMirrored ? (rotationAngle - Math.PI) % (2 * Math.PI) : rotationAngle % (2 * Math.PI);
                // поворачиваем контур
                bottomRight = _geometryService.RotatePointAroundAxis(bottomRight, locationPoint, XYZ.BasisZ.ToDomain(), rotationAngle);
                bottomLeft = _geometryService.RotatePointAroundAxis(bottomLeft, locationPoint, XYZ.BasisZ.ToDomain(), rotationAngle);
                topRight = _geometryService.RotatePointAroundAxis(topRight, locationPoint, XYZ.BasisZ.ToDomain(), rotationAngle);
                topLeft = _geometryService.RotatePointAroundAxis(topLeft, locationPoint, XYZ.BasisZ.ToDomain(), rotationAngle);                
                center=_geometryService.RotatePointAroundAxis(center, locationPoint, XYZ.BasisZ.ToDomain(), rotationAngle);


                if (!isMirrored)
                {
                    Contour contour = new Contour()
                    {
                        Top = new DetailLineDomain(topRight, topLeft),
                        Left = new DetailLineDomain(topLeft, bottomLeft),
                        Bottom = new DetailLineDomain(bottomLeft, bottomRight),
                        Right = new DetailLineDomain(topRight,bottomRight),                        
                        Center = center

                    };
                    return contour;
                }
                else
                {
                    Contour contour = new Contour()
                    {
                        Top = new DetailLineDomain(topLeft, topRight),
                        Left = new DetailLineDomain(topRight, bottomRight),
                        Bottom = new DetailLineDomain(bottomRight, bottomLeft),
                        Right = new DetailLineDomain(topLeft,bottomLeft),                        
                        Center = center

                    };
                    return contour;
                }
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure<Contour>(ex.Message);
            }
        }
        public CSharpFunctionalExtensions.Result<List<string>> DrawContour(Contour contour,string familyName)
        {
            var resultParameters = _jsonConfig.GetParameterNamesForFamily(familyName);
            if (resultParameters.IsFailure)
                return CSharpFunctionalExtensions.Result.Failure<List<string>>(resultParameters.Error);

            List<DetailLineDomain> contourLines = new List<DetailLineDomain>();
            contourLines.Add(contour.Left);
            contourLines.Add(contour.Bottom);
            contourLines.Add(contour.Right);
            var result=_drawLineService.DrawLines("рисование 6H0 контура",contourLines);
            if (result.IsSuccess)
                return result.Value;
            else
                return result;
            
        }
    }
}
