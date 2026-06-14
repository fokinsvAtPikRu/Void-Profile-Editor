using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Void_Profile_Editor.Domain.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.DTOs
{
    public class AllowedFamiliesConfigDto 
    {
        [JsonProperty("AllowedFamilyNames")]
        public List<AllowedFamilyDto> AllowedFamiliesNames { get; set; }
    }
    public class AllowedFamilyDto
    {
        [JsonProperty("FamilyName")]
        public string FamilyName { get; set; }
        [JsonProperty("Parameters")]
        public FamilyParametersDto Parameters { get; set; }
    }

    public class AllowedFamilies
    {
        public List<AllowedFamilyDto> AllowedFamilyNames { get; set; }
    }
    public class FamilyParametersDto
    {
        [JsonProperty("DoubleParameters")]
        public Dictionary<string,double> DoubleParameters { get; set; }
        [JsonProperty("IntParameters")]
        public Dictionary<string, int> IntParameters { get; set; }

    }
}
