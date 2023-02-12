using Models;

namespace Workflow.GameProcessors;

public class GameProcessorBase
{
    public GameProcessorBase()
    {

    }
    
    public List<GameStats> SortEntries(IEnumerable<GameStats> gameStatsList)
    {
        _logger.Debug("Sorting entries.");
        return gameStatsList.OrderByDescending(r => r.Score).ThenByDescending(r => r.FieldKills).ToList();
    }
}