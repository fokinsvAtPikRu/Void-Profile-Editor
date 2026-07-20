using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Void_Profile_Editor.Domain.Abstraction.Configuration;
using Void_Profile_Editor.Domain.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.DTOs;

namespace Void_Profile_Editor.Infrastructure.Configuration
{
    public class FamilyParameterNameConfig : IParameterNameConfig
    {
        private readonly string _familyName;
        private readonly Dictionary<string, string> _parameterMap;
        private readonly HashSet<string> _allParameterNames;
        private readonly ParameterDictionary _defaultValues;
        private readonly Dictionary<ContourSideName, SideMappingDto> _sideMappings;

        private FamilyParameterNameConfig(string familyName, Parameters familyConfig)
        {
            _familyName = familyName;
            _parameterMap = new Dictionary<string, string>();
            _allParameterNames = new HashSet<string>();
            _defaultValues = new ParameterDictionary();
            _sideMappings = new Dictionary<ContourSideName, SideMappingDto>();

            BuildMappings(familyConfig);
            BuildDefaultValues(familyConfig);
        }
        public static Result<FamilyParameterNameConfig> Create(string familyName, Parameters familyConfig)
        {
            // проверка для метода BuildMappings()
            if (familyConfig == null)
                return Result.Failure<FamilyParameterNameConfig>("familyConfig is null");
            if (familyConfig.ParameterMappings == null)
                return Result.Failure<FamilyParameterNameConfig>("familyConfig.ParameterMappings is null");
            foreach (var mapping in familyConfig.ParameterMappings)
            {               
                if (mapping.Value == null)
                    return Result.Failure<FamilyParameterNameConfig>($"{mapping.Key} : Value is null");
            }
            // проверка для метода BuildDefaultValues()            
            if (familyConfig.DoubleParameters == null)
                return Result.Failure<FamilyParameterNameConfig>("familyConfig.DoubleParameters is null");
            if (familyConfig.IntParameters == null)
                return Result.Failure<FamilyParameterNameConfig>("familyConfig.IntParameters is null");

            return Result.Success<FamilyParameterNameConfig>(new FamilyParameterNameConfig(familyName,familyConfig));
        }
        private void BuildMappings(Parameters familyConfig)
        {
            foreach (var mapping in familyConfig.ParameterMappings)
            {
                if (Enum.TryParse<ContourSideName>(mapping.Key, true, out var side))
                {
                    _sideMappings[side] = mapping.Value;
                    AddToParameterMap(side, mapping.Value);                    
                }
            }            
        }
        private Result AddToParameterMap(ContourSideName side, SideMappingDto mapping)
        {
            if (mapping == null)
                return Result.Failure($"for {side} side mapping == null");

            var sideKey = side.ToString().ToLower();

            AddMapping($"{sideKey}_enabled", mapping.Enabled);
            AddMapping($"{sideKey}_offset_start", mapping.OffsetStart);
            AddMapping($"{sideKey}_offset_end", mapping.OffsetEnd);
            AddMapping($"{sideKey}_hole_offset", mapping.HoleOffset);
            AddMapping($"{sideKey}_hole_width", mapping.HoleWidth);
            return Result.Success();
        }

        private void AddMapping(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _parameterMap[key] = value;
                _allParameterNames.Add(value);
            }
        }

        private Result BuildDefaultValues(Parameters familyConfig)
        {
            if (familyConfig == null)
                return Result.Failure("familyConfig is null");
            if (familyConfig.DoubleParameters == null)
                return Result.Failure("familyConfig.DoubleParameters is null");
            if (familyConfig.IntParameters == null)
                return Result.Failure("familyConfig.IntParameters is null");

            foreach (var paramName in familyConfig.DoubleParameters)
            {
                _defaultValues.DoubleParameters[paramName] = 0.0;
            }

            foreach (var paramName in familyConfig.IntParameters)
            {
                _defaultValues.IntParameters[paramName] = 1;
            }
            return Result.Success();
        }

        public string GetParameterName(ParameterRole role, ContourSideName side, bool isStart = false)
        {
            var sideKey = side.ToString().ToLower();
            var roleKey = role.ToString().ToLower();

            if (role == ParameterRole.OffsetStart || role == ParameterRole.OffsetEnd)
            {
                roleKey = isStart ? "offset_start" : "offset_end";
            }

            var key = $"{sideKey}_{roleKey}";
            return _parameterMap.TryGetValue(key, out var name) ? name : null;
        }

        public IReadOnlyCollection<string> GetAllParameterNames()
        {
            return _allParameterNames.ToList().AsReadOnly();
        }

        public ParameterDictionary GetDefaultValues()
        {
            return _defaultValues;
        }

        public SideMappingDto GetSideMapping(ContourSideName side)
        {
            return _sideMappings.TryGetValue(side, out var mapping) ? mapping : null;
        }

        public IReadOnlyList<ContourSideName> GetAvailableSides()
        {
            return _sideMappings.Keys.ToList().AsReadOnly();
        }

        public bool HasParameter(string parameterName)
        {
            return _allParameterNames.Contains(parameterName);
        }
    }

    public enum ParameterRole
    {
        Enabled,
        OffsetStart,
        OffsetEnd,
        HoleOffset,
        HoleWidth,
        Custom
    }
}

