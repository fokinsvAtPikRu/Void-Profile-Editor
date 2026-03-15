using Autodesk.Revit.UI;
using System;
using System.Linq;
using Autodesk.Revit.DB;
using Void_Profile_Editor.Abstraction;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            ObservableCollection<ElementId> createdLineIds = null)
        {

            if (transaction == null)
                throw new InvalidOperationException("Метод может вызван только внутри транзакции");
            if (view == null)
                view = _document.ActiveView;
            DetailLine detailLine = _document.Create.NewDetailCurve(view, line) as DetailLine;
            if (createdLineIds != null)
                createdLineIds.Add(detailLine.Id);
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

        public CSharpFunctionalExtensions.Result DeleteLines(
                ObservableCollection<ElementId> lineIds,
                Transaction transaction = null)
        {
            // Проверка транзакции
            if (transaction == null)
                return CSharpFunctionalExtensions.Result.Failure("Метод может быть вызван только внутри транзакции");

            // Проверка списка
            if (lineIds == null || lineIds.Count == 0)
                return CSharpFunctionalExtensions.Result.Success(); // Нечего удалять - считаем успехом

            // Проверка документа
            if (_document == null)
                return CSharpFunctionalExtensions.Result.Failure("Документ не инициализирован");

            try
            {
                // Фильтруем только валидные элементы (которые еще существуют в документе)
                var validIds = lineIds
                    .Where(id => id != null && id.IntegerValue != -1 && _document.GetElement(id) != null)
                    .ToList();

                if (validIds.Count == 0)
                    return CSharpFunctionalExtensions.Result.Success(); // Все элементы уже удалены

                // Удаляем элементы
                _document.Delete(validIds);
                lineIds.Clear();                

                return CSharpFunctionalExtensions.Result.Success();
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure($"Ошибка при удалении линий: {ex.Message}");
            }
        }
    }
}
