using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Void_Profile_Editor.Domain.Abstraction.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.DTOs;

namespace Void_Profile_Editor.Infrastructure.Configuration
{
    public class JsonFamilyConfigService : IAllowedFamiliesConfig
    {
        private readonly string _configPath;
        private Dictionary<string, AllowedFamilyDto> _familyConfigs = new Dictionary<string, AllowedFamilyDto>();
        private Dictionary<string, PressureContourParameters> _familyParameters = new Dictionary<string, PressureContourParameters>();
        private readonly object _lock = new object();

        public JsonFamilyConfigService(string configPath = null)
        {
            _configPath = configPath ?? GetDefaultConfigPath();
            LoadConfig();
        }

        public int Count => _familyConfigs.Count;
        private string GetDefaultConfigPath()
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyLocation);
            var configPath = Path.Combine(directory, "Infrastructure", "Configuration");
            if (Directory.Exists(configPath))
                configPath = Path.Combine(directory, "FamilyTypesConfig.json");
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

                    var newConfigs = new Dictionary<string, AllowedFamilyDto>();
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

        public PressureContourParameters GetParametersForFamily(string familyName)
        {
            if (TryGetParametersForFamily(familyName, out var parameters))
                return parameters;

            return new PressureContourParameters(familyName);
        }

        public bool TryGetParametersForFamily(string familyName, out PressureContourParameters parameters)
        {
            parameters = null;

            if (string.IsNullOrEmpty(familyName))
                return false;

            return _familyParameters?.TryGetValue(familyName, out parameters) ?? false;
        }

        public IEnumerable<string> GetAllDoubleParameters()
        {
            if (_familyParameters == null)
                return Enumerable.Empty<string>();

            return _familyParameters.Values
                .SelectMany(p => p.DoubleParameters?.Keys ?? Enumerable.Empty<string>())
                .Distinct()
                .OrderBy(name => name);
        }

        public IEnumerable<string> GetAllIntParameters()
        {
            if (_familyParameters == null)
                return Enumerable.Empty<string>();

            return _familyParameters.Values
                .SelectMany(p => p.IntParameters?.Keys ?? Enumerable.Empty<string>())
                .Distinct()
                .OrderBy(name => name);
        }
        public AllowedFamilyDto GetFamilyConfig(string familyName)
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
