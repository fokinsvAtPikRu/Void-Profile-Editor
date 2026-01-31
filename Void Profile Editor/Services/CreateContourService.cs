using Autodesk.Revit.DB;
using System;
using Void_Profile_Editor.Model;
using Void_Profile_Editor.Abstraction;

namespace Void_Profile_Editor.Services
{
    public class CreateContourService :ICreateContourService
    {
        private Document _document;
        private IGeometryService _geometryService;
        private IDrawLineService _drawLineService;
        
        public CreateContourService(Document document, IGeometryService geometryService, IDrawLineService drawLineService) 
        {
            _document = document;
            _geometryService = geometryService;
            _drawLineService=drawLineService;
        }
        public CSharpFunctionalExtensions.Result<Contour> Create
            (XYZ locationPoint,
            double rotationAngle,
            double h0,
            double thickness,
            double offset,
            bool isMirrored)
        {
            try
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
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure<Contour>(ex.Message);
            }
        }
        public CSharpFunctionalExtensions.Result DrawContour(Contour contour)
        {
            try
            {
                using (var t = new Transaction(_document, "Построение контура"))
                {
                    t.Start();

                    _drawLineService.DrawLine(line: contour.Bottom, transaction: t);
                    _drawLineService.DrawLine(line: contour.Left, transaction: t);
                    _drawLineService.DrawLine(line: contour.Right, transaction: t);


                    t.Commit();
                }
                return CSharpFunctionalExtensions.Result.Success();
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure(ex.Message);
            }
        }
    }
}
