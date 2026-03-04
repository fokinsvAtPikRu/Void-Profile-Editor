using Autodesk.Revit.UI;
using System;
using System.Linq;
using Autodesk.Revit.DB;
using Void_Profile_Editor.Abstraction;
using System.Collections.Generic;

namespace Void_Profile_Editor.Services
{
    public class DrawLineService : IDrawLineService
    {
        private Document _document;
        public DrawLineService(Document document)
        {
            _document = document;
        }
        public CSharpFunctionalExtensions.Result DrawLine(Line line, 
            Transaction transaction = null, 
            View view = null, 
            string lineStyleName = "Тонкие линии",
            List<ElementId> createdElementId = null)
        {
            
            if (transaction == null)
                throw new InvalidOperationException("Метод может вызван только внутри транзакции");
            if (view == null)
                view = _document.ActiveView;            
            DetailLine detailLine = _document.Create.NewDetailCurve(view, line) as DetailLine;
            if (detailLine != null)
            {
                GraphicsStyle lineStyle = GetLineStyleByName(_document, lineStyleName);
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
