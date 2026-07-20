using Newtonsoft.Json;
using System.Collections.Generic;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.DTOs
{
    public class RootObject 
    {
        [JsonProperty("AllowedFamilyNames")]
        public List<AlowwedFamilyWrapperDto> AllowedFamilyNames { get; set; }
    }
    public class AlowwedFamilyWrapperDto
    {
        [JsonProperty("FamilyName")]
        public string FamilyName { get; set; }
        [JsonProperty("FamilyType")]
        public string FamilyType { get; set; }

        [JsonProperty("Parameters")]
        public Parameters Parameters { get; set; }
    }
    public class Parameters
    {        
        [JsonProperty("ActiveEdge")]
        public List<ContourSideName> ActiveEdge {  get; set; }
        [JsonProperty("Dimensions")]
        public Dimensions Dimensions {  get; set; }

        [JsonProperty("DoubleParameters")]
        public List<string> DoubleParameters { get; set; }

        [JsonProperty("IntParameters")]
        public List<string> IntParameters { get; set; }

        [JsonProperty("ParameterMappings")]
        public Dictionary<string,SideMappingDto> ParameterMappings {  get; set; }
    }
    public class Dimensions
    {
        [JsonProperty("DoubleParameters")]
        public List<string> DoublrParameters { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }

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
