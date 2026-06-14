using CSharpFunctionalExtensions;
using Void_Profile_Editor.Domain.Abstraction.Services;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Domain.Services;
using Void_Profile_Editor.UserCases.Abstraction;
using Void_Profile_Editor.UserCases.Results;

namespace Void_Profile_Editor.UserCases.Cases
{
    public class CreateCountourUseCase : ICreateContourUserCase
    {
        private readonly ICreateContourService _createContourService;

        public CreateCountourUseCase(ICreateContourService createContourService)
        {
            _createContourService = createContourService;
        }
        public Result<ResultCreateContourUserCase> CreateContour(ResultSelectInstanceUserCase resultSelectInstanceUseCase)
        {
            if (resultSelectInstanceUseCase == null)
                return Result.Failure<ResultCreateContourUserCase>("ResultSelectInstanceUseCase resultSelectInstanceUseCase == null");
            if (resultSelectInstanceUseCase.PressureContour == null)
                return Result.Failure<ResultCreateContourUserCase>("Контур продавливания не создан");
            PressureContour pressureContour = resultSelectInstanceUseCase.PressureContour;
            var resultCreate6H0Contour = Create6H0Contour(pressureContour);
            if (resultCreate6H0Contour.IsFailure)
                return Result.Failure<ResultCreateContourUserCase>(resultCreate6H0Contour.Error);
            var resutDrawContour = _createContourService.DrawContour(resultCreate6H0Contour.Value);
            if (resutDrawContour.IsFailure)
                return Result.Failure<ResultCreateContourUserCase>(resutDrawContour.Error);
            var resultCreateHalfH0Contour = CreateHalfH0Contour(pressureContour);
            if (resultCreateHalfH0Contour.IsFailure)
                return Result.Failure<ResultCreateContourUserCase>(resultCreateHalfH0Contour.Error);
            return new ResultCreateContourUserCase
            {
                Contour6H0 = resultCreate6H0Contour.Value,
                ContourHalfH0 = resultCreateHalfH0Contour.Value,
                LinesIdsForDelete = resutDrawContour.Value
            };
        }
        private Result<Contour> Create6H0Contour(PressureContour pressureContour)
        {
            return _createContourService.Create(
                pressureContour.InsertPoint,
                pressureContour.Rotation,
                pressureContour.ContourParameters.DoubleParameters["h0"],
                pressureContour.ContourParameters.DoubleParameters["Толщина"],
                6 * pressureContour.ContourParameters.DoubleParameters["h0"],
                pressureContour.IsMirrored).Value;
            
        }
        private Result<Contour> CreateHalfH0Contour(PressureContour pressureContour)
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
