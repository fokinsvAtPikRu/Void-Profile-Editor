using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Abstraction.Services;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Abstraction;

namespace Void_Profile_Editor.Infrastructure.Services
{
    internal class RevitSelectionServices : IRevitSelectionServices
    {
        private readonly RevitTask _revitTask;
        private readonly ISelectionService _selectionService;

        public RevitSelectionServices(
            RevitTask revitTask,
            ISelectionService selectionService
            )
        {
            _revitTask = revitTask;

        }



        public Task<Result<FamilyInstance>> PickFamilyInstanceAsync(string prompt = "Выберите то")
        {
            throw new NotImplementedException();
        }

        public async Task<CSharpFunctionalExtensions.Result<Point3DDomain>> PickPointAsync(string prompt = "Выберите точку")
        {
            return await _revitTask.Run<Point3DDomain>(app => _selectionService.PickPoint());
        }
    }
}
