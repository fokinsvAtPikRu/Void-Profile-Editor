using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Void_Profile_Editor.Infrastructure.Abstraction;

namespace Void_Profile_Editor.UseCases
{
    public class SelectInstanceUseCase : ISelectInstanceUseCase
    {
        IRevitSelectionServices _revitSelectionService;

        public SelectInstanceUseCase(
            IRevitSelectionServices revitSelectionService)
        {
            _revitSelectionService = revitSelectionService;
        }
        public async Task<Result<ResultSelectInstanceUseCase>> RunAsync()
        {
            
            var result = await _revitSelectionService.PickFamilyInstanceAsync("Выберете семейство");
            if (result.IsSuccess)
            {
                return new ResultSelectInstanceUseCase
                {
                    PressureContour = result.Value
                };
            }
            else
            {
                return Result.Failure<ResultSelectInstanceUseCase>("Объект не выбран");
            }            
        }
    }
}
