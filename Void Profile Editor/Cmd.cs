using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Void_Profile_Editor.Services;

namespace Void_Profile_Editor
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            // выбираем семейство продавливания
            var selectionService = new SelectionService(commandData);
            FamilyInstance element = selectionService.PickObject();

            // получаем информацию о семействе
            var pressureCountourService = new PressureCounturInformationService(commandData, new GeometryService());
            var pressureContour = pressureCountourService.CreatePressureConturInfo(element);

            // создаем контур на расстоянии 6h0 от площади на которую действует продавливающая сила            
            var createContourService = new CreateContourService(new GeometryService());
            var contour6h0 = createContourService.Create(
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
                pressureContour.Value.H0,
                element.Mirrored);


            // рисуем контур
            using (var t=new Transaction(doc,"Контуры"))
            {
                t.Start();
                var drawLineService=new DrawLineService(commandData);
                drawLineService.DrawLine(line:contour6h0.Value.Bottom,transaction: t);
                drawLineService.DrawLine(line: contour6h0.Value.Left, transaction: t);
                drawLineService.DrawLine(line: contour6h0.Value.Right, transaction: t);
                drawLineService.DrawLine(line: contourH0.Value.Right, transaction: t);
                drawLineService.DrawLine(line: contourH0.Value.Right, transaction: t);
                drawLineService.DrawLine(line: contourH0.Value.Right, transaction: t);

                t.Commit();
            }

            // выбирааем точку
            XYZ point = selectionService.PickPoint();
            // строим отсекующую линию 
            Line cutingLine = Line.CreateBound(point, (pressureCountourService.GetCenterPressureContur(pressureContour.Value)).Value);
            // ищем пересечения
            foreach (var line in contourH0.Value)
            {
                line.Value.Intersect(cutingLine);
            }

            
            //TaskDialog.Show("debug", debugInfo);



            return Result.Succeeded;
        }

        
    }
}
