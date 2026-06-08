using CSharpFunctionalExtensions;
using System.Linq;
using Void_Profile_Editor.Domain.Abstraction.Services;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.UserCases.Abstraction;

namespace Void_Profile_Editor.UserCases.Cases
{
    public class CreateCuttingLinesUserCase : ICreateCuttingLinesUserCase
    {
        private readonly ISelectionService _selectionService;
        private readonly IRevitLineService _drawLineService;
        private readonly IGeometryService _geometryService;
        private readonly IRevitUpdateParametersService _pressureCounturInformationService;
        public CreateCuttingLinesUserCase(
            ISelectionService selectionService,
            IRevitLineService drawLineService,
            IGeometryService geometryService,
            IRevitUpdateParametersService pressureCounturInformationService)
        {
            _selectionService = selectionService;
            _drawLineService = drawLineService;
            _geometryService = geometryService;
            _pressureCounturInformationService = pressureCounturInformationService;
        }
        public Result CreateCuttingLines(Contour contourHalfH0, PressureContour pressureContour)
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
            var drawLineResult = _drawLineService.DrawLines("Секущие линии для контура продавливания", cuttingLines.ToList());
            if (drawLineResult.IsFailure)
                return Result.Failure(drawLineResult.Error);
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
                return Result.Failure(resultUpdateParameters.Error);
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
