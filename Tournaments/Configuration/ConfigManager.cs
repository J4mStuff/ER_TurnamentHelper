using System.Data;
using System.Text.Json;

namespace Tournaments.Configuration;

public class ConfigManager
{
    private const string ConfigFilename = "config.json";

    public ConfigurationModel ReadAllSettings()
    {
        var configString = File.Exists(ConfigFilename)
            ? File.ReadAllText(ConfigFilename)
            : throw new NoNullAllowedException("Configuration file missing");

        return JsonSerializer.Deserialize<ConfigurationModel>(configString)
               ?? new ConfigurationModel(new List<string>());
    }
}