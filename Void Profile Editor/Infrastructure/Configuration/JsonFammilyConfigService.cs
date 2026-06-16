using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Void_Profile_Editor.Domain.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.DTOs;

namespace Void_Profile_Editor.Infrastructure.Configuration
{
    public class JsonFammilyConfigService : IAllowedFamiliesConfig
    {
        private readonly string _configPath;        
        private Dictionary<string,PressureContourParameters> _familyParameters;
        private readonly object _lock = new object();
        
        public JsonFammilyConfigService(string configPath = null)
        {
            _configPath = configPath ?? GetDefaultConfigPath();
            var result =LoadConfig();            
        }

        public int Count => _familyParameters.Count;
        private string GetDefaultConfigPath()
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyLocation);
            var configPath = Path.Combine(directory, "Infrastructure", "Configuration", "FamilyTypesConfig.json");
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
                        _familyParameters = new Dictionary<string,PressureContourParameters>();
                        return Result.Failure($"Config file not found: {_configPath}");
                    }

                    var json = File.ReadAllText(_configPath);
                    var dto = JsonConvert.DeserializeObject<AllowedFamiliesConfigDto>(json);

                    var newParameters = new Dictionary<string, PressureContourParameters>();
                    if (dto?.AllowedFamiliesNames != null)
                    {
                        foreach (var family in dto.AllowedFamiliesNames)
                        {
                            if (!string.IsNullOrEmpty(family?.FamilyName))
                            {
                                var parameters = new PressureContourParameters(
                                    family.FamilyName,
                                    family.Parameters?.DoubleParameters ?? new Dictionary<string, double>(),
                                    family.Parameters?.IntParameters ?? new Dictionary<string, int>());
                                newParameters[family.FamilyName] = parameters;
                            }                                
                        }
                    }
                    _familyParameters = newParameters;
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
            return !string.IsNullOrEmpty(familyName) && _familyParameters.ContainsKey(familyName);
        }

        public IReadOnlyList<string> GetAllowedFamilies()
        {
            return _familyParameters.Keys.ToList().AsReadOnly();
        }


        public Result Reload() => LoadConfig();

        public PressureContourParameters GetParametersForFamily()
        {
            throw new NotImplementedException();
        }

        public bool TryGetParametersForFamily(string familyName, out PressureContourParameters parameters)
        {
            return _familyParameters.TryGetValue(familyName, out parameters);
        }
    }
}
