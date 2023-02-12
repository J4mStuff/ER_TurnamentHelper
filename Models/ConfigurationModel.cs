
// ReSharper disable CollectionNeverUpdated.Global

namespace Models;

public class ConfigurationModel
{
    public ConfigurationModel()
    {
        PlayerSeparator = string.Empty;
        GameFiles = new List<string>();
        ColumnIds = new GameFieldIds();
        GameTypeSwitch = new Dictionary<string, bool>();
    }

    public List<string> GameFiles { get; set; }

    public GameFieldIds ColumnIds { get; set; }
    public Dictionary<string, bool> GameTypeSwitch { get; set; }
    public string PlayerSeparator { get; set; }

    public IEnumerable<string> GetTrackedModes()
    {
        return GameTypeSwitch.Where(g => g.Value).Select(g => g.Key);
    }
}