using Models;

namespace Workflow.GameProcessors;

public class SoloGameProcessor : GameProcessorBase
{
    public void GenerateData(IList<List<GameStats>> games,
        ModeConfigurationModel modeConfiguration, PointDeductions pointDeductions,
        string separator)
    {
        Logger.Debug($"Processing {modeConfiguration.Name} mode.");
        var lastGame = StupidClone.PerformStupidClone(games.Last());

        ProcessSoloGame(lastGame, modeConfiguration, pointDeductions);
        GenerateSoloSummaryData(games, modeConfiguration, pointDeductions);
    }

    private void GenerateSoloSummaryData(IEnumerable<List<GameStats>> games,
        ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        var processedGames = new List<GameStats>();
        foreach (var game in games)
        {
            processedGames.AddRange(ProcessSingleGame(game, modeConfigurationModel));
        }
        
        processedGames.ForEach(g => g.CalculateKillScoreWithDeductions(modeConfigurationModel.KillMultiplier, 
            pointDeductions.GetPlayerDeduction(g.PlayerName)));
        
        processedGames = SortEntries(processedGames);
        Logger.Debug($"Got {processedGames.Count} entries for game summary");
        
        ImageDrawer.PopulateSoloTemplate(processedGames, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        Logger.Info("Summary processing complete.");
    }
    
    private void ProcessSoloGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel,
        PointDeductions pointDeductions)
    {
        gameStatsList = ProcessSingleGame(gameStatsList, modeConfigurationModel);
        gameStatsList.ForEach(g => g.CalculateKillScoreWithDeductions(modeConfigurationModel.KillMultiplier, 
            pointDeductions.GetPlayerDeduction(g.PlayerName)));

        gameStatsList = SortEntries(gameStatsList);

        ImageDrawer.PopulateSoloTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        Logger.Info("Last game processing complete.");
    }
}