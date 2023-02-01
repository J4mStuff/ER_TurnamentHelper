using Tournaments.Workflow;

namespace Tournaments.Models;

public class ConfigurationModel
{
    public ConfigurationModel()
    {
        GameFiles = new List<string>();
        ColumnIds = new GameFieldIds();
        GameTypeSwitch = new Dictionary<string, bool>();
        ModeMaps = new List<ModeConfiguration>();
    }

    public List<string> GameFiles { get; set; }
    public GameFieldIds ColumnIds { get; set; }
    public Dictionary<string, bool> GameTypeSwitch { get; set; }
    public List<ModeConfiguration> ModeMaps { get; set; }

}