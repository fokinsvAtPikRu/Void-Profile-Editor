using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Void_Profile_Editor.Domain.Abstraction.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.DTOs;
using Void_Profile_Editor.Infrastructure.Abstraction;

namespace Void_Profile_Editor.Infrastructure.Configuration
{
    public class JsonFamilyConfigService : IAllowedFamiliesConfig
    {
        private readonly string _configPath;
        private Dictionary<string, AlowwedFamilyWrapperDto> _familyConfigs = new Dictionary<string, AlowwedFamilyWrapperDto>();
        //private Dictionary<string, PressureContourParameters> _familyParameters = new Dictionary<string, PressureContourParameters>();
        private IRevitMessageService _revitMessageService;
        private readonly object _lock = new object();

        public JsonFamilyConfigService(
            IRevitMessageService revitMessageService,
            string configPath = null)
        {
            _revitMessageService = revitMessageService;
            _configPath = configPath ?? GetDefaultConfigPath();
            var result = LoadConfig();
            if (result.IsFailure)
                _revitMessageService.ShowMessage("Error", result.Error);
        }

        public int Count => _familyConfigs.Count;
        private string GetDefaultConfigPath()
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyLocation);
            var configPath = Path.Combine(directory, "Infrastructure", "Configuration");
            if (Directory.Exists(configPath))
                configPath = Path.Combine(configPath, "FamilyTypesConfig.json");
            return configPath;
        }
        private Result LoadConfig()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_configPath))
                    {
                        return Result.Failure($"Config file not found: {_configPath}");
                    }

                    var json = File.ReadAllText(_configPath);
                    var dto = JsonConvert.DeserializeObject<AllowedFamiliesConfigDto>(json);

                    var newConfigs = new Dictionary<string, AlowwedFamilyWrapperDto>();
                    if (dto?.AllowedFamilyNames != null)
                    {
                        foreach (var family in dto.AllowedFamilyNames)
                        {
                            if (family != null && !string.IsNullOrEmpty(family?.FamilyName))
                            {
                                newConfigs[family.FamilyName] = family;
                            }
                        }
                    }
                    _familyConfigs = newConfigs;
                }
                catch (Exception ex)
                {
                    return Result.Failure(ex.ToString());
                }
                return Result.Success();
            }
        }

        public bool IsAllowed(string familyName)
        {
            return !string.IsNullOrEmpty(familyName) && _familyConfigs.ContainsKey(familyName);
        }

        public IReadOnlyList<string> GetAllowedFamilies()
        {
            return _familyConfigs.Keys.ToList().AsReadOnly();
        }


        public void Reload() => LoadConfig();


        public Result<PressureContourParameters> GetParameterNamesForFamily(string familyName)
        {
            if (string.IsNullOrEmpty(familyName))
                return Result.Failure<PressureContourParameters>("familyName is null or empty");
            // check ActiveEdge
            if (_familyConfigs[familyName].Parameters.ActiveEdge == null)
                return Result.Failure<PressureContourParameters>("ActiveEdge is null");
            if (_familyConfigs[familyName].Parameters.ActiveEdge.Count == 0)
                return Result.Failure<PressureContourParameters>("ActiveEdge is empty");
            // check DoubleParameters
            if (_familyConfigs[familyName].Parameters.DoubleParameters == null)
                return Result.Failure<PressureContourParameters>("DoubleParameters is null");
            if (_familyConfigs[familyName].Parameters.DoubleParameters.Count == 0)
                return Result.Failure<PressureContourParameters>("DoubleParameters is empty");
            // check IntParameters
            if (_familyConfigs[familyName].Parameters.IntParameters == null)
                return Result.Failure<PressureContourParameters>("IntParameters is null");
            if (_familyConfigs[familyName].Parameters.IntParameters.Count == 0)
                return Result.Failure<PressureContourParameters>("IntParameters is empty");
            // заполняем параметры 
            var activeEdge = new List<ContourSideName>();
            foreach (var name in _familyConfigs[familyName].Parameters.ActiveEdge)
            {
                activeEdge.Add(name);
            }
            var doubleParameters = new Dictionary<string, double>();
            foreach (var p in _familyConfigs[familyName].Parameters.DoubleParameters)
            {
                doubleParameters.Add(p, 0.0);
            }
            var intParameters = new Dictionary<string, int>();
            foreach (var p in _familyConfigs[familyName].Parameters.IntParameters)
            {
                intParameters.Add(p, 0);
            }
            return new PressureContourParameters(familyName, activeEdge, doubleParameters, intParameters);
        }

        public AlowwedFamilyWrapperDto GetFamilyConfig(string familyName)
        {
            return _familyConfigs.TryGetValue(familyName, out var config) ? config : null;
        }

        public AllowedFamiliesConfigDto GetFullConfig()
        {
            return new AllowedFamiliesConfigDto
            {
                AllowedFamilyNames = _familyConfigs.Values.ToList()
            };
        }
    }
}
