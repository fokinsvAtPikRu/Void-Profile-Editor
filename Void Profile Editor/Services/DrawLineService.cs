using Autodesk.Revit.UI;
using System;
using System.Linq;
using Autodesk.Revit.DB;

namespace Void_Profile_Editor.Services
{
    public class DrawLineService
    {
        private ExternalCommandData _commandData;
        public DrawLineService(ExternalCommandData commandData)
        {
            _commandData = commandData;
        }
        public CSharpFunctionalExtensions.Result DrawLine(Line line, Transaction transaction = null, View view = null, string lineStyleName = "Тонкие линии")
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;
            if (transaction == null)
                throw new InvalidOperationException("Метод может вызван только внутри транзакции");
            if (view == null)
                view = doc.ActiveView;            
            DetailLine detailLine = doc.Create.NewDetailCurve(view, line) as DetailLine;
            if (detailLine != null)
            {
                GraphicsStyle lineStyle = GetLineStyleByName(doc, lineStyleName);
                if (lineStyle != null)
                {
                    detailLine.LineStyle = lineStyle;
                }
            }
            return CSharpFunctionalExtensions.Result.Success();
        }
        private GraphicsStyle GetLineStyleByName(Document doc, string styleName)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle))
                .WhereElementIsNotElementType()
                .Cast<GraphicsStyle>()
                .FirstOrDefault(gs => gs.Name == styleName);
        }
    }
}
