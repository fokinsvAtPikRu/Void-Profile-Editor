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
        private Result<ResultCreateContourUseCase> CreateContour(ResultSelectInstanceUseCase instance)
        {
            if (instance == null)            
                return Result.Failure<ResultCreateContourUseCase>("Семейство не выбрано");
            
            if (instance.PressureContour == null)
                return Result.Failure<ResultCreateContourUseCase>("Контур продавливания не создан");
            var result = Create6H0Contour().
                        Bind(c => DrawContour(c)).
                        Bind(() => CreateHalfH0Contour());
            if (result.IsFailure)
                TaskDialog.Show("Test", $"Error:{result.Error}");
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
        private CSharpFunctionalExtensions.Result<Contour> CreateHalfH0Contour()
        {
            ContourHalfH0 = _createContourService.Create(
               _pressureContour.InsertPoint,
               _pressureContour.Rotation,
               _pressureContour.ContourParameters.DoubleParameters["h0"],
               _pressureContour.ContourParameters.DoubleParameters["Толщина"],
               0.5 * _pressureContour.ContourParameters.DoubleParameters["h0"],
               _instance.Mirrored).Value;
            return CSharpFunctionalExtensions.Result.Success<Contour>(ContourHalfH0);
        }
    }
}
