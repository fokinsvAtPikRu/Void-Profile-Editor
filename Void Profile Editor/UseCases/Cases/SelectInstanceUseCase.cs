using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.UserCases.Abstraction;
using Void_Profile_Editor.UserCases.Results;

namespace Void_Profile_Editor.UserCases.Cases
{
    public class SelectInstanceUseCase : ISelectInstanceUserCase
    {
        IRevitSelectionServices _revitSelectionService;

        public SelectInstanceUseCase(IRevitSelectionServices revitSelectionService)
        {
            _revitSelectionService = revitSelectionService;
        }
        public async Task<Result<ResultSelectInstanceUserCase>> RunAsync()
        {

            var result = await _revitSelectionService.PickFamilyInstanceAsync("Выберете семейство");
            if (result.IsSuccess)
            {
                return new ResultSelectInstanceUserCase
                {
                    PressureContour = result.Value
                };
            }
            else
            {
                return Result.Failure<ResultSelectInstanceUserCase>(result.Error);
            }
        }
    }
}
