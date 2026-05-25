using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Adapters;

namespace Void_Profile_Editor.Infrastructure.Services
{
    public class RevitSelectionServices : IRevitSelectionServices
    {
        private readonly RevitTask _revitTask;
        private readonly ISelectionService _selectionService;

        public RevitSelectionServices(
            RevitTask revitTask,
            ISelectionService selectionService
            )
        {
            _revitTask = revitTask;
            _selectionService = selectionService;
        }



        public async Task<Result<PressureContour>> PickFamilyInstanceAsync(string prompt = "Выберите объект")
        {
            var result = await _revitTask.Run(app => _selectionService.PickObject());
            return result.Value.ToDomain();
        }

        public async Task<Result<Point3DDomain>> PickPointAsync(string prompt = "Выберите точку")
        {
            var result= await _revitTask.Run(app => _selectionService.PickPoint());
            return result;
        }
    }
}
