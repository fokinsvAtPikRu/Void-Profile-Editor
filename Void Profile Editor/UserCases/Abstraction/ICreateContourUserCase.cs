using CSharpFunctionalExtensions;
using Void_Profile_Editor.UserCases.Results;

namespace Void_Profile_Editor.UserCases.Abstraction
{
    public interface ICreateContourUserCase
    {
        Result<ResultCreateContourUserCase> CreateContour(ResultSelectInstanceUserCase resultSelectInstanceUseCase);
    }
}
