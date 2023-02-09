using System.Configuration;
using System.Data;
using System.Text.Json;
using Logger;
using Models;

namespace Configuration;

// ReSharper disable once ClassNeverInstantiated.Global
public class ConfigManager
{
    private const string ConfigFilename = "config.json";
    private readonly CustomLogger _logger;

    public ConfigManager()
    {
        _logger = new CustomLogger();
    }
    
    public ConfigurationModel ReadAllSettings()
    {
        var configString = File.Exists(ConfigFilename)
            ? File.ReadAllText(ConfigFilename)
            : throw new NoNullAllowedException("Configuration file missing");
        
        _logger.Debug("Retrieved configuration file.");

        var model = JsonSerializer.Deserialize<ConfigurationModel>(configString) ??
                    throw new ConfigurationErrorsException("Cannot parse configuration file.");

        return model;
    }
}