using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.UseCases.Results;

namespace Void_Profile_Editor.UseCases.Abstraction
{
    public interface ISelectInstanceUseCase
    {
        Task<Result<ResultSelectInstanceUseCase>> RunAsync();
    }
}
