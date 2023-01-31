using Tournaments.Workflow;

namespace Tournaments.Configuration;

public class ConfigurationModel
{
    public ConfigurationModel(List<string> gameFiles)
    {
        GameFiles = gameFiles;
    }

    public List<string> GameFiles { get; set; }
    public GameFieldIds ColumnIds { get; set; }
    public Dictionary<string, bool> GameTypeSwitch { get; set; }
    public int GameCount => GameFiles.Count;
}