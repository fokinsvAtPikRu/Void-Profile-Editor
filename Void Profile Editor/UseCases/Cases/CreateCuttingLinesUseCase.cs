
using CSharpFunctionalExtensions;
using System;
using System.Linq;
using Void_Profile_Editor.Domain.Abstraction.Services;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;

namespace Void_Profile_Editor.UseCases.Cases
{
    public class CreateCuttingLinesUseCase
    {
        private readonly ISelectionService _selectionService;
        private readonly IDrawLineService _drawLineService;
        private readonly IGeometryService _geometryService;
        private readonly IPressureCounturInformationService _pressureCounturInformationService;
        public CreateCuttingLinesUseCase(
            ISelectionService selectionService,
            IDrawLineService drawLineService,
            IGeometryService geometryService,
            IPressureCounturInformationService pressureCounturInformationService)
        {
            _selectionService = selectionService;
            _drawLineService = drawLineService;
            _geometryService = geometryService;
            _pressureCounturInformationService = pressureCounturInformationService;
        }
        private void CreateCuttingLines(Contour contourHalfH0, PressureContour pressureContour)
        {
            if (contourHalfH0 == null) 

            try
            {




                var cuttingLines = new Line[2];
                // указываем первую точку для создания секущей линии 
                _selectionService.PickPoint()
                    // обнуляем координату Z у точки 
                    .Bind((point) =>
                    {
                        return CSharpFunctionalExtensions.Result.Success(new XYZ(point.X, point.Y, 0));
                    })
                    // строим первую секущую линию
                    .Bind((point) =>
                    {
                        if (contourHalfH0 == null)
                            return CSharpFunctionalExtensions.Result.Failure("Контур 0,5H0 не создан");
                        cuttingLines[0] = Line.CreateBound(point, contourHalfH0.Center);
                        return CSharpFunctionalExtensions.Result.Success(cuttingLines);
                    })
                    // повторяем, строим вторую секущую линию
                    .Bind(() => _selectionService.PickPoint())
                    .Bind((point) =>
                    {
                        return CSharpFunctionalExtensions.Result.Success(new XYZ(point.X, point.Y, 0));
                    })
                    .Bind((point) =>
                    {
                        cuttingLines[1] = Line.CreateBound(point, contourHalfH0.Center);
                        return CSharpFunctionalExtensions.Result.Success(cuttingLines);
                    })
                    // ищем точки пересечения секущих линий с контуром 0.5H0
                    .Bind((lines) => FindIntersection(contourHalfH0, lines))
                    // упорядочиваем _intersectionPoints
                    .Bind((points) =>
                    {
                        if (points[0] == null || points[1] == null)
                            return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>("Не заданы секущие линии");
                        return points
                            .OrderBy(p => p.SideName)
                            .ThenBy(p => p.Point.DistanceTo(contourHalfH0.GetLine(p.SideName).GetEndPoint(0)))
                            .ToArray();

                    })
                    // вычисляем параметры
                    .Bind((points) => _geometryService.CalculateParameters(contourHalfH0, points, pressureContour))
                    // сохраняем параметры
                    .Bind(() => _pressureCounturInformationService.UpdateParameters(_document, _instance, _pressureContour.ContourParameters));
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Error:{ex.Message}");
            }
        }
        private Result<IntersectionPoint[]> FindIntersection(Contour contourHalfH0, Line[] cuttingLines)
        {
            if (contourHalfH0 == null)
                return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>("Контур 0,5H0 не создан");
            if (cuttingLines == null)
                return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>("Секущие линии не созданы");
            var result = _geometryService.LineWithContourIntersection(cuttingLines, contourHalfH0);
            if (result.IsSuccess)
                return result.Value;
            else
                return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>("Точки пересечения не найдены");

        }
    }
}
