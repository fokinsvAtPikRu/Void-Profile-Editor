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
            var selectionFIlter = new SelectionService(commandData);
            FamilyInstance element = selectionFIlter.PickObject();

            // получаем информацию о семействе
            var pressureCountourService = new PressureCounturInformationService(commandData);
            var pressureContour = pressureCountourService.CreatePressureConturInfo(element);

            // создаем контур на расстоянии 6h0 от площади на которую действует продавливающая сила
            var create6h0ContourService = new Create6H0ContourService();
            var contour6h0 = create6h0ContourService.Create(
                pressureContour.Value.InsertPoint,
                pressureContour.Value.Rotation,
                pressureContour.Value.H0,
                pressureContour.Value.WallThickness,
                element.Mirrored);

            // рисуем контур
            using(var t=new Transaction(doc,"Контур 6h0"))
            {
                t.Start();
                var drawLineService=new DrawLineService(commandData);
                drawLineService.DrawLine(line:contour6h0.Value.Bottom,transaction: t);
                drawLineService.DrawLine(line: contour6h0.Value.Left, transaction: t);
                drawLineService.DrawLine(line: contour6h0.Value.Right, transaction: t);

                t.Commit();
            }
            
            //TaskDialog.Show("debug", debugInfo);



            return Result.Succeeded;
        }

        private HashSet<Line> GetAllLines(GeometryElement geomElement, HashSet<Line> lines = null)
        {
            if (lines == null)
                lines = new HashSet<Line>();
            foreach (GeometryObject geometryObject in geomElement)
            {
                TaskDialog.Show("DEBUG", $"{typeof(GeometryObject)}");
                if (geometryObject is Line line)
                    lines.Add(line);
                else if (geometryObject is GeometryInstance geometryInstance)
                    GetAllLines(geometryInstance.GetInstanceGeometry(), lines);
            }
            return lines;
        }
    }
}
