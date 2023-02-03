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

    public List<ModeConfiguration> GetTrackedModes()
    {
        var toReturn = new List<ModeConfiguration>();
        foreach (var mode in ModeMaps)
        {
            if (GameTypeSwitch.ContainsKey(mode.Name) && GameTypeSwitch[mode.Name])
            {
                toReturn.Add(mode);
            }
        }

        return toReturn;
    }
}