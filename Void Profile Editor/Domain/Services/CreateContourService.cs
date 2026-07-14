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

        public CreateContourService(
            IGeometryService geometryService, 
            IRevitLineService drawLineService)         
        {            
            _geometryService = geometryService;
            _drawLineService=drawLineService;            
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
        public CSharpFunctionalExtensions.Result<List<string>> DrawContour(string trnsactionName, Contour contour, List<ContourSideName> activeEdge)
        {  
            List<DetailLineDomain> contourLines = new List<DetailLineDomain>();
            foreach (var edge in activeEdge)
            {
                switch (edge)
                {
                    case ContourSideName.Left:
                        contourLines.Add(contour.Left);
                        break;
                    case ContourSideName.Right:
                        contourLines.Add(contour.Right);
                        break;
                    case ContourSideName.Top:
                        contourLines.Add(contour.Top);
                        break;
                    case ContourSideName.Bottom:
                        contourLines.Add(contour.Bottom);
                        break;
                }
                    
            }
            var result=_drawLineService.DrawLines(trnsactionName, contourLines);
            if (result.IsSuccess)
                return result.Value;
            else
                return CSharpFunctionalExtensions.Result.Failure<List<string>>( result.Error);
            
        }
    }
}
