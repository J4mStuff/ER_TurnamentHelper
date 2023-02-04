using System.Configuration;
using System.Data;
using System.Text.Json;
using Serilog;
using Tournaments.Models;

namespace Tournaments.Configuration;

public static class ConfigManager
{
    private const string ConfigFilename = "config.json";

    public static ConfigurationModel ReadAllSettings()
    {
        var configString = File.Exists(ConfigFilename)
            ? File.ReadAllText(ConfigFilename)
            : throw new NoNullAllowedException("Configuration file missing");
        
        Log.Debug("Retrieved configuration file.");

        var model = JsonSerializer.Deserialize<ConfigurationModel>(configString) ??
                    throw new ConfigurationErrorsException("Cannot parse configuration file.");

        return model;
    }
}