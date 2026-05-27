using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Adapters;

namespace Void_Profile_Editor.Infrastructure.Services
{
    public class DrawLineService : IDrawLineService
    {
        private Document _document;
        public DrawLineService(Document document)
        {
            _document = document;
        }
        public CSharpFunctionalExtensions.Result<List<string>> DrawLine(
            string trMessage,
            List<DetailLineDomain> linesDomain,
            View view = null,
            string lineStyleName = "Тонкие линии"
            )
        {
            if (linesDomain == null || linesDomain.Count() == 0)
                return CSharpFunctionalExtensions.Result.Failure<List<string>>("Список линий не создан или пуст");

            if (view == null)
                view = _document.ActiveView;

            GraphicsStyle lineStyle = GetLineStyleByName(_document, lineStyleName);
            List<string> createdLinesIdsDomain = new List<string>();

            using (Transaction tr = new Transaction(_document, trMessage))
            {
                foreach (var line in linesDomain)
                {
                    DetailLine revitlLine = _document.Create.NewDetailCurve(view, line.ToRevit()) as DetailLine;
                    if (revitlLine != null)
                    {
                        revitlLine.LineStyle = lineStyle;
                        createdLinesIdsDomain.Add(revitlLine.Id.ToDomain());
                    }
                }
            }
            if (linesDomain.Count == createdLinesIdsDomain.Count)
                return createdLinesIdsDomain;
            else
                return CSharpFunctionalExtensions.Result.Failure<List<string>>(
                    $"Не удалось создать {linesDomain.Count - createdLinesIdsDomain.Count} линий");
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
