using Newtonsoft.Json;
using System.Collections.Generic;

namespace Void_Profile_Editor.DTOs
{
    public class AllowedFamiliesConfigDto 
    {
        [JsonProperty("AllowedFamilyNames")]
        public List<AllowedFamilyDto> AllowedFamilyNames { get; set; }
    }
    public class AllowedFamilyDto
    {
        [JsonProperty("FamilyName")]
        public string FamilyName { get; set; }

        [JsonProperty("DoubleParameters")]
        public List<string> DoubleParameters { get; set; }

        [JsonProperty("IntParameters")]
        public List<string> IntParameters { get; set; }

        [JsonProperty("ParameterMappings")]
        public Dictionary<string,SideMappingDto> ParameterMappings {  get; set; }
    }
    public class SideMappingDto
    {
        [JsonProperty("Enabled")]
        public string Enabled { get; set; }

        [JsonProperty("OffsetStart")]
        public string OffsetStart { get; set; }

        [JsonProperty("OffsetEnd")]
        public string OffsetEnd { get; set; }

        [JsonProperty("HoleOffset")]
        public string HoleOffset { get; set; }

        [JsonProperty("HoleWidth")]
        public string HoleWidth { get; set; }       
    }
}
