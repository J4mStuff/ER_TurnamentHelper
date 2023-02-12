using Models;

namespace Workflow.GameProcessors;

public class TagGameProcessor : GameProcessorBase
{
    public void GenerateTagSummaryData(IEnumerable<List<GameStats>> games,
        ModeConfigurationModel modeConfigurationModel, CustomTeams teams, PointDeductions pointDeductions,
        string separator)
    {
        var processedGames = new List<GameStats>();
        foreach (var game in games)
        {
            processedGames.AddRange(ProcessSingleTagGame(game, teams, modeConfigurationModel, separator));
        }

        processedGames = processedGames.GroupBy(x => x.TeamName).Select(g
            => ProcessSummaryTeamGroup(g, separator)).ToList();
        processedGames.ForEach(g => g.CalculateKillScoreWithDeductions(modeConfigurationModel.KillMultiplier, 
            pointDeductions.GetPlayerDeduction(g.PlayerName)));
        
        processedGames = SortEntries(processedGames);
        Logger.Debug($"Got {processedGames.Count} entries for game summary");

        ImageDrawer.PopulateTeamTemplate(processedGames, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        Logger.Info("Summary processing complete.");
    }

    public void ProcessTagGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel,
        CustomTeams teams, PointDeductions pointDeductions, string separator)
    {
        gameStatsList = ProcessSingleTagGame(gameStatsList, teams, modeConfigurationModel, separator);
        gameStatsList.ForEach(g => g.CalculateKillScoreWithDeductions(modeConfigurationModel.KillMultiplier, 
            pointDeductions.GetPlayerDeduction(g.PlayerName)));

        gameStatsList = SortEntries(gameStatsList);

        ImageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        Logger.Info("Last game processing complete.");
    }

    private List<GameStats> ProcessSingleTagGame(List<GameStats> gameStatsList, CustomTeams teams, ModeConfigurationModel modeConfigurationModel, string separator)
    {
        gameStatsList.ForEach(r => r.TeamName = teams.GetPlayerTeam(r.PlayerName) ?? r.TeamName);
        gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(g => ProcessSingleGameTeamGroup(g, separator))
            .ToList();
        gameStatsList.ForEach(r => r.UpdateScoreWithPlacement(modeConfigurationModel.PlacementScoring));
        Logger.Debug($"Got {gameStatsList.Count} entries for last game");

        return gameStatsList;
    }
}