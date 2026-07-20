using System.Threading.Tasks;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using Void_Profile_Editor.Domain.Abstraction.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Adapters;
using Void_Profile_Editor.Infrastructure.Model;

namespace Void_Profile_Editor.Infrastructure.Services
{
    public class RevitSelectionServices : IRevitSelectionServices
    {
        private readonly RevitTask _revitTask;
        private IAllowedFamiliesConfig _config;
        private readonly ISelectionService _selectionService;

        public RevitSelectionServices(
            RevitTask revitTask,
            IAllowedFamiliesConfig config,
            ISelectionService selectionService
            )
        {
            _revitTask = revitTask;
            _config = config;
            _selectionService = selectionService;
        }



        public async Task<Result<RevitPressureContour>> PickFamilyInstanceAsync(string prompt = "Выберите объект")
        {
            var result = await _revitTask.Run(app => _selectionService.PickObject());
            if (result.IsSuccess)
            {
                var instance = result.Value;
                var resultToDomain = instance.ToDomain(_config);
                if (resultToDomain.IsFailure)
                    return Result.Failure<RevitPressureContour>(resultToDomain.Error);
                return resultToDomain.Value;
            }
            else
                return Result.Failure<RevitPressureContour>(result.Error);
        }

        public async Task<Result<Point3DDomain>> PickPointAsync(string prompt = "Выберите точку")
        {
            var result= await _revitTask.Run(app => _selectionService.PickPoint());
            return result;
        }
    }
}
