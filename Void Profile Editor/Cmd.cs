using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RxBim.Command.Revit;
using System;
using System.Collections.Generic;
using System.Linq;
using Void_Profile_Editor.Services;

namespace Void_Profile_Editor
{
    [Transaction(TransactionMode.Manual)]
    public class Cmd : RxBimCommand
    {
        public Result Execute(IServiceProvider provider)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // выбираем семейство продавливания
            var selectionService = new SelectionService(commandData);
            FamilyInstance element = selectionService.PickObject();
            if (element == null)
                return Result.Cancelled;

            // получаем информацию о семействе
            var pressureCountourService = new PressureCounturInformationService(commandData, new GeometryService());
            var pressureContour = pressureCountourService.CreatePressureContourInfo(element);

            // создаем контур на расстоянии 6h0 от площади на которую действует продавливающая сила            
            var createContourService = new CreateContourService(new GeometryService());
            var contour6H0 = createContourService.Create(
                pressureContour.Value.InsertPoint,
                pressureContour.Value.Rotation,
                pressureContour.Value.H0,
                pressureContour.Value.WallThickness,
                6 * pressureContour.Value.H0,
                element.Mirrored);
            var contourH0 = createContourService.Create(
                pressureContour.Value.InsertPoint,
                pressureContour.Value.Rotation,
                pressureContour.Value.H0,
                pressureContour.Value.WallThickness,
                pressureContour.Value.H0 / 2,
                element.Mirrored);


            // рисуем контур
            using (var t = new Transaction(doc, "Контуры"))
            {
                t.Start();
                var drawLineService = new DrawLineService(commandData);
                drawLineService.DrawLine(line: contour6H0.Value.Bottom, transaction: t);
                drawLineService.DrawLine(line: contour6H0.Value.Left, transaction: t);
                drawLineService.DrawLine(line: contour6H0.Value.Right, transaction: t);
                drawLineService.DrawLine(line: contourH0.Value.Bottom, transaction: t);
                drawLineService.DrawLine(line: contourH0.Value.Right, transaction: t);
                drawLineService.DrawLine(line: contourH0.Value.Left, transaction: t);

                t.Commit();
            }

            // выбирааем точку
            XYZ point = selectionService.PickPoint();
            if (point == null)
                return Result.Cancelled;
            var pointNullZ = new XYZ(
                point.X,
                point.Y,
                0);
            // строим отсекающую линию 
            Line cutingLine = Line.CreateBound(pointNullZ, (pressureCountourService.GetCenterPressureContur(pressureContour.Value)).Value);
            using (var t = new Transaction(doc, "Секущая линия"))
            {
                t.Start();
                var drawLineService = new DrawLineService(commandData);
                drawLineService.DrawLine(line: cutingLine, transaction: t);
                t.Commit();
            }

            // ищем пересечения
            GeometryService  geometryService = new GeometryService();
            string sideContour;
            geometryService.LineWithContourIntersection(cutingLine, contourH0.Value, out sideContour);


            //TaskDialog.Show("debug", debugInfo);



            return Result.Succeeded;
        }


    }
}
