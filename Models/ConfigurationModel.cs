
// ReSharper disable CollectionNeverUpdated.Global

namespace Models;

public class ConfigurationModel
{
    public ConfigurationModel()
    {
        PlayerSeparator = "|";
        GameFiles = new List<string>();
        ColumnIds = new GameFieldIds();
        GameTypeSwitch = new Dictionary<string, bool>();
        ModeMaps = new List<ModeConfiguration>();
    }

    public List<string> GameFiles { get; set; }

    public GameFieldIds ColumnIds { get; set; }
    public Dictionary<string, bool> GameTypeSwitch { get; set; }
    public List<ModeConfiguration> ModeMaps { get; set; }
    public string PlayerSeparator { get; set; }

    public List<ModeConfiguration> GetTrackedModes()
    {
        return ModeMaps.Where(mode => GameTypeSwitch.ContainsKey(mode.Name) && GameTypeSwitch[mode.Name]).ToList();
    }
}