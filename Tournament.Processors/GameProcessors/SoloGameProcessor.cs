using Models;

namespace Workflow.GameProcessors;

public class SoloGameProcessor : GameProcessorBase
{
    public SoloGameProcessor()
    {
        
    }
    
    public void ProcessSoloGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        gameStatsList.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for last game");
        
        _imageDrawer.PopulateSoloTemplate(gameStatsList, modeConfigurationModel, modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        _logger.Info("Last game processing complete.");
    }
    
    public void GenerateSoloSummaryData(IEnumerable<List<GameStats>> games, ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        var gameStatsList = entries.GroupBy(x => x.PlayerName).Select(ProcessSoloGroup).ToList();
        
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateSoloTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        _logger.Info("Summary processing complete.");
    }
    
    private GameStats ProcessSoloGroup(IGrouping<string,GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();
        _logger.Debug($"Got {temp.Count} player entries for {grouping.Key}");

        var main = temp.First();
        foreach (var item in temp.Skip(1))
        {
            main.FieldKills += item.FieldKills;
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
        }
        _logger.Debug($"Player {main.PlayerName}'s stats are processed");

        return main;
    }
}