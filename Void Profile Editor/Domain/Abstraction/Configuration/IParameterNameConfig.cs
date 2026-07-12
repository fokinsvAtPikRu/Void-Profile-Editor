using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Void_Profile_Editor.Domain.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.DTOs;
using Void_Profile_Editor.Infrastructure.Configuration;

namespace Void_Profile_Editor.Domain.Abstraction.Configuration
{
    public interface IParameterNameConfig
    {
        public string GetParameterName(ParameterRole role, ContourSideName side, bool isStart = false);

        public IReadOnlyCollection<string> GetAllParameterNames();

        public ParameterDictionary GetDefaultValues();

        public SideMappingDto GetSideMapping(ContourSideName side);


        public IReadOnlyList<ContourSideName> GetAvailableSides();


        public bool HasParameter(string parameterName);
        
    }
}
