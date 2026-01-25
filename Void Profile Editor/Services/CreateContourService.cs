using Autodesk.Revit.DB;
using System;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Services
{
    public class CreateContourService
    {
        private GeometryService _geometryService;
        public CreateContourService(GeometryService geometryService) 
        {
            _geometryService = geometryService;
        }
        public CSharpFunctionalExtensions.Result<Contour> Create
            (XYZ locationPoint,
            double rotationAngle,
            double h0,
            double thickness,
            double offset,
            bool isMirrored)
        {
            // вычисляем координаты углов контура
            XYZ bottomRight = new XYZ
                (locationPoint.X + thickness / 2 + offset,
                locationPoint.Y - offset,
                0);
            XYZ bottomLeft = new XYZ
                (locationPoint.X - thickness / 2 - offset,
                locationPoint.Y - offset,
                0);
            XYZ topRight = new XYZ
                (locationPoint.X + thickness / 2 + offset,
                locationPoint.Y + thickness + 0.5 * h0,
                0);
            XYZ topLeft = new XYZ
                (locationPoint.X - thickness / 2 - offset,
                locationPoint.Y + thickness + 0.5 * h0,
                0);
            rotationAngle = isMirrored ? (rotationAngle - Math.PI) % (2 * Math.PI) : rotationAngle % (2 * Math.PI);
            // поворачиваем контур
            bottomRight = _geometryService.RotatePointAroundAxis(bottomRight, locationPoint, XYZ.BasisZ, rotationAngle);            
            bottomLeft = _geometryService.RotatePointAroundAxis(bottomLeft, locationPoint, XYZ.BasisZ, rotationAngle);
            topRight = _geometryService.RotatePointAroundAxis(topRight, locationPoint, XYZ.BasisZ, rotationAngle);
            topLeft = _geometryService.RotatePointAroundAxis(topLeft, locationPoint, XYZ.BasisZ, rotationAngle);
            Contour contour = new Contour()
            {
                Bottom = Line.CreateBound(bottomRight, bottomLeft),
                Left = Line.CreateBound(topLeft, bottomLeft),
                Right = Line.CreateBound(topRight, bottomRight)
            };
            return contour;
        }        
    }
}
