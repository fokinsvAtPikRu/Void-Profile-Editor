using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.UseCases
{
    public interface ISelectInstanceUseCase
    {
        Task<Result<ResultSelectInstanceUseCase>> RunAsync();
    }
}
