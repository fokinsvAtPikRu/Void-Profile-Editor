using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Void_Profile_Editor.UserCases.Results;

namespace Void_Profile_Editor.UserCases.Abstraction
{
    public interface ISelectInstanceUserCase
    {
        Task<Result<ResultSelectInstanceUserCase>> RunAsync();
    }
}
