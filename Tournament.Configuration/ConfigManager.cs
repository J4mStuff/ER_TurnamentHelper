using System.Configuration;
using System.Text.Json;
using Logger;
using Models;

namespace Configuration;

public class ConfigManager
{
    private readonly CustomLogger _logger;

    public ConfigManager(CustomLogger logger)
    {
        _logger = logger;
    }

    public ConfigurationModel ReadMainConfig()
    {
        return GetConfig<ConfigurationModel>("config.json");
    }

    public ModeConfigurationModel ReadModeConfig(string name)
    {
        var path = Path.Combine("modes", $"{name.ToLower()}.json");
        return GetConfig<ModeConfigurationModel>(path);
    }

    private T GetConfig<T>(string fileName)
    {
        string configString;
        try
        {
            configString = File.ReadAllText(fileName);
        }
        catch (Exception ex)
        {
            _logger.Fatal($"Failed to retrieve configuration '{fileName}' with exception: {ex.Message}");
            throw;
        }

        _logger.Debug($"Retrieved configuration file '{fileName}'.");

        var model = JsonSerializer.Deserialize<T>(configString) ??
                    throw new ConfigurationErrorsException("Cannot parse configuration file.");

        return model;
    }
}