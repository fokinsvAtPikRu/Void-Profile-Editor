using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Void_Profile_Editor.Domain.Configuration;

namespace Void_Profile_Editor.Infrastructure.Configuration
{
    public class JsonFammilyConfigService : IAllowedFamiliesConfig
    {
        private readonly string _configPath;        
        private HashSet<string> _allowedFamilyNames;
        private readonly object _lock = new object();
        private bool _isConfigLoaded = false;

        public IReadOnlyCollection<string> AllowedFamilyNames { get; private set; }

        public IReadOnlyCollection<string> AllowedFamilies => throw new NotImplementedException();

        public event Action<IReadOnlyCollection<string>> OnConfigChanged;

        public JsonFammilyConfigService(string configPath = null)
        {
            _configPath = configPath ?? GetDefaultConfigPath();
            var result =LoadConfig();
            _isConfigLoaded = result.IsSuccess;
        }

        private string GetDefaultConfigPath()
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyLocation);
            var configPath = Path.Combine(directory, "Configuration", "FamilyTypesConfig.json");
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
                        return Result.Failure($"Config file not found: {_configPath}");

                    var json = File.ReadAllText(_configPath);
                    var dto = JsonConvert.DeserializeObject<AllowedFamilies>(json);

                    var newNames = new HashSet<string>();
                    if (dto?.AllowedFamilyNames != null)
                    {
                        foreach (var family in dto.AllowedFamilyNames)
                        {
                            if (!string.IsNullOrEmpty(family?.FamilyName))
                                newNames.Add(family.FamilyName);
                        }
                    }

                    var oldNames = _allowedFamilyNames;
                    _allowedFamilyNames = newNames;
                    AllowedFamilyNames = new ReadOnlyCollection<string>(newNames.ToList());

                    if (oldNames != null && !oldNames.SetEquals(newNames))
                    {
                        OnConfigChanged?.Invoke(AllowedFamilyNames);
                    }
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
            if (string.IsNullOrEmpty(familyName)) 
                return false;
            return _allowedFamilyNames?.Contains(familyName) ?? false;
        }

        public Result Reload() => LoadConfig();
        
    }
}
