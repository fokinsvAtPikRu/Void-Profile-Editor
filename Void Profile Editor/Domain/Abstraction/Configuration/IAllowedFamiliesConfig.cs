using CSharpFunctionalExtensions;
using System.Collections.Generic;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Domain.Abstraction.Configuration
{
    public interface IAllowedFamiliesConfig
    {
        int Count { get; }
        bool IsAllowed(string familyName);
        IReadOnlyList<string> GetAllowedFamilies();
        Result<PressureContourParameters> GetParameterNamesForFamily(string familyName);
        
        
    }
}
