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
            game.ForEach(r => r.TeamName = teams.GetPlayerTeam(r.PlayerName) ?? r.TeamName);
            var processedGame = game.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
            processedGame.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring,
                modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

            processedGames.AddRange(processedGame);
        }

        processedGames = processedGames.GroupBy(x => x.TeamName).Select(g
            => ProcessSummaryTeamGroup(g, separator)).ToList();
        processedGames = SortEntries(processedGames);
        Logger.Debug($"Got {processedGames.Count} entries for game summary");

        ImageDrawer.PopulateTeamTemplate(processedGames, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        Logger.Info("Summary processing complete.");
    }

    public void ProcessTagGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel,
        CustomTeams teams, PointDeductions pointDeductions, string separator)
    {
        gameStatsList.ForEach(r => r.TeamName = teams.GetPlayerTeam(r.PlayerName) ?? r.TeamName);
        gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(g => ProcessLastGameTeamGroup(g, separator))
            .ToList();
        gameStatsList.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring,
            modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        gameStatsList = SortEntries(gameStatsList);
        Logger.Debug($"Got {gameStatsList.Count} entries for last game");

        ImageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        Logger.Info("Last game processing complete.");
    }
}