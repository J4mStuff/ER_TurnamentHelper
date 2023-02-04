using System.Configuration;
using System.Data;
using System.Text.Json;
using Tournaments.Models;

namespace Tournaments.Configuration;

public class ConfigManager
{
    private const string ConfigFilename = "config.json";

    public static ConfigurationModel ReadAllSettings()
    {
        var configString = File.Exists(ConfigFilename)
            ? File.ReadAllText(ConfigFilename)
            : throw new NoNullAllowedException("Configuration file missing");

        var model = JsonSerializer.Deserialize<ConfigurationModel>(configString) ??
                    throw new ConfigurationErrorsException("Cannot parse configuration file.");

        return model;
    }
}