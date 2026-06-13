using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace Void_Profile_Editor.Domain.Configuration
{
    public interface IAllowedFamiliesConfig
    {
        IReadOnlyCollection<string> AllowedFamilies { get; }
        bool IsAllowed(string familyName);
        Result Reload();
        event Action<IReadOnlyCollection<string>> OnConfigChanged;
    }
    public class AllowedFamily
    {
        public string FamilyName { get; set; }
    }

    public class AllowedFamilies
    {
        public List<AllowedFamily> AllowedFamilyNames { get; set; }
    }
}
