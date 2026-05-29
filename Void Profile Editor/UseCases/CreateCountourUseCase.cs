using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Domain.Services;
using Void_Profile_Editor.Infrastructure.Adapters;

namespace Void_Profile_Editor.UseCases
{
    public class CreateCountourUseCase
    {
        private readonly CreateContourService _createContourService;

        public CreateCountourUseCase(CreateContourService createContourService)
        {
            _createContourService = createContourService;
        }
        private Result<ResultCreateContourUseCase> CreateContour(ResultSelectInstanceUseCase resultSelectInstanceUseCase)
        {
            if (resultSelectInstanceUseCase == null)
                return Result.Failure<ResultCreateContourUseCase>("ResultSelectInstanceUseCase resultSelectInstanceUseCase == null");
            if (resultSelectInstanceUseCase.PressureContour == null)
                return Result.Failure<ResultCreateContourUseCase>("Контур продавливания не создан");
            PressureContour pressureContour = resultSelectInstanceUseCase.PressureContour;
            var resultCreate6H0Contour = Create6H0Contour(pressureContour);
            if (resultCreate6H0Contour.IsFailure)
                return Result.Failure<ResultCreateContourUseCase>(resultCreate6H0Contour.Error);
            var resutDrawContour = _createContourService.DrawContour(resultCreate6H0Contour.Value);
            if (resutDrawContour.IsFailure)
                return Result.Failure<ResultCreateContourUseCase>(resutDrawContour.Error);
            var resultCreateHalfH0Contour = CreateHalfH0Contour(pressureContour);
            if (resultCreateHalfH0Contour.IsFailure)
                return Result.Failure<ResultCreateContourUseCase>(resultCreateHalfH0Contour.Error);
            return new ResultCreateContourUseCase
            {
                Contour6H0 = resultCreate6H0Contour.Value,
                ContourHalfH0 = resultCreateHalfH0Contour.Value,
                LinesIdsForDelete = resutDrawContour.Value
            };
        }
        private CSharpFunctionalExtensions.Result<Contour> Create6H0Contour(PressureContour pressureContour)
        {
            return _createContourService.Create(
                pressureContour.InsertPoint,
                pressureContour.Rotation,
                pressureContour.ContourParameters.DoubleParameters["h0"],
                pressureContour.ContourParameters.DoubleParameters["Толщина"],
                6 * pressureContour.ContourParameters.DoubleParameters["h0"],
                pressureContour.IsMirrored).Value;
            
        }
        private CSharpFunctionalExtensions.Result<Contour> CreateHalfH0Contour(PressureContour pressureContour)
        {
            return _createContourService.Create(
               pressureContour.InsertPoint,
               pressureContour.Rotation,
               pressureContour.ContourParameters.DoubleParameters["h0"],
               pressureContour.ContourParameters.DoubleParameters["Толщина"],
               0.5 * pressureContour.ContourParameters.DoubleParameters["h0"],
               pressureContour.IsMirrored).Value;
            
        }
    }
}
