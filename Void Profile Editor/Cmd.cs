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
            // ищем вложеноое семейство штриховки
            var nestedInstance = new FilteredElementCollector(element.Document)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(fi => fi.Symbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBasedHosted)
                .Where(fi => fi.Host != null /*&& fi.Host.Id == element.Id && fi.Name.Contains("Торец стены")*/)
                .ToList();
                //.FirstOrDefault();
            TaskDialog.Show("Debug", $"{nestedInstance.First().Host.Id}");
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
