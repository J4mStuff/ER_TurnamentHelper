using Models;

namespace Workflow.GameProcessors;

public class SquadGameProcessor : GameProcessorBase
{
    public void GenerateData(IList<List<GameStats>> games,
        ModeConfigurationModel modeConfiguration, PointDeductions pointDeductions,
        string separator)
    {
        Logger.Debug($"Processing {modeConfiguration.Name} mode.");
        var lastGame = StupidClone.PerformStupidClone(games.Last());

        ProcessSquadGame(lastGame, modeConfiguration, pointDeductions, separator);
        GenerateSquadSummaryData(games, modeConfiguration, pointDeductions, separator);
    }

    private void GenerateSquadSummaryData(IEnumerable<List<GameStats>> games,
        ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions, string separator)
    {
        var processedGames = new List<GameStats>();
        foreach (var game in games)
        {
            processedGames.AddRange(ProcessSingleTeamGame(game, modeConfigurationModel, separator));
        }
            
        processedGames = processedGames.GroupBy(x => x.TeamName).Select(g
            => ProcessSummaryTeamGroup(g, separator)).ToList();
        processedGames.ForEach(g => g.CalculateTeamKillScoreWithDeductions(modeConfigurationModel.KillMultiplier, 
            pointDeductions.GetPlayerDeduction(g.PlayerName)));
        
        processedGames = SortEntries(processedGames);
        Logger.Debug($"Got {processedGames.Count} entries for game summary");
            
        ImageDrawer.PopulateTeamTemplate(processedGames, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        Logger.Info("Summary processing complete.");
    }
    
    private void ProcessSquadGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel,
        PointDeductions pointDeductions, string separator)
    {
        gameStatsList = ProcessSingleTeamGame(gameStatsList, modeConfigurationModel, separator);
        gameStatsList.ForEach(g => g.CalculateTeamKillScoreWithDeductions(modeConfigurationModel.KillMultiplier, 
            pointDeductions.GetPlayerDeduction(g.PlayerName)));

        gameStatsList = SortEntries(gameStatsList);

        ImageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        Logger.Info("Last game processing complete.");
    }
}