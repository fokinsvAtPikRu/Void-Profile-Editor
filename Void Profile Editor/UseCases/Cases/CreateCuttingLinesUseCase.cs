
using Autodesk.Revit.DB;
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
        private Result CreateCuttingLines(Contour contourHalfH0, PressureContour pressureContour)
        {
            if (contourHalfH0 == null)
                return Result.Failure("Контур 0.5h0 не создан");
            if (pressureContour == null)
                return Result.Failure("Контур продавливания не создан");

            var cuttingLines = new DetailLineDomain[2];
            for (var i = 0; i < cuttingLines.Length; i++)
            {
                var resultPickPoint = _selectionService.PickPoint();
                if (resultPickPoint.IsFailure)
                    return Result.Failure(resultPickPoint.Error);
                Point3DDomain point = new Point3DDomain(
                    resultPickPoint.Value.X,
                    resultPickPoint.Value.Y,
                    0);
                cuttingLines[i] = new DetailLineDomain(point, contourHalfH0.Center);
            }
            var resutIntersectionPoints = FindIntersection(contourHalfH0, cuttingLines);
            if (resutIntersectionPoints.IsFailure)
                return Result.Failure(resutIntersectionPoints.Error);
            var orderedPoints = resutIntersectionPoints.Value
                .OrderBy(p => p.SideName)
                .ThenBy(p => p.Point.DistanceTo(contourHalfH0.GetLine(p.SideName).Start))
                .ToArray();
            _geometryService.CalculateParameters(contourHalfH0, orderedPoints, pressureContour);
            var resultUpdateParameters = _pressureCounturInformationService.UpdateParameters(pressureContour);
            if (resultUpdateParameters.IsFailure)
                return Result.Failure("не удалось обновить параметры");
            return Result.Success();           
        }
        private Result<IntersectionPoint[]> FindIntersection(Contour contourHalfH0, DetailLineDomain[] cuttingLines)
        {
            if (contourHalfH0 == null)
                return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>("Контур 0,5H0 не создан");
            if (cuttingLines == null)
                return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>("Секущие линии не созданы");
            var result = _geometryService.LineWithContourIntersection(cuttingLines, contourHalfH0);
            if (result.IsSuccess)
                return result;
            else
                return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>($"Ошибка {result.Error}");

        }
    }
}
