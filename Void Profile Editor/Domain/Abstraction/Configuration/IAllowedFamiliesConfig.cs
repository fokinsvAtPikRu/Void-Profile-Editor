using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Domain.Abstraction.Configuration
{
    public interface IAllowedFamiliesConfig
    {
        int Count { get; }
        bool IsAllowed(string familyName);
        IReadOnlyList<string> GetAllowedFamilies();

        PressureContourParameters GetParametersForFamily(string familyName);
        bool TryGetParametersForFamily(string familyName, out PressureContourParameters parameters);
        IEnumerable<string> GetAllDoubleParameters();
        IEnumerable<string> GetAllIntParameters();
    }
}
