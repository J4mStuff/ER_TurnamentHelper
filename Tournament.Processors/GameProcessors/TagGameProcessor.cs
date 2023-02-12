using Models;
using Workflow.Spreadsheet;

namespace Workflow.GameProcessors;

public class TagGameProcessor : GameProcessorBase
{
    private readonly TemporaryTeamSpreadsheet _temporaryTeamSpreadsheet;

    public TagGameProcessor()
    {
        _temporaryTeamSpreadsheet = new TemporaryTeamSpreadsheet();
    }

    public void GenerateData(IList<List<GameStats>> games,
        ModeConfigurationModel modeConfiguration, PointDeductions pointDeductions,
        string separator)
    {
        Logger.Debug($"Processing {modeConfiguration.Name} mode.");
        var lastGame = StupidClone.PerformStupidClone(games.Last());
        var teams = _temporaryTeamSpreadsheet.ProcessTemporaryTeams();
        var tagGameProcessor = new TagGameProcessor();
        tagGameProcessor.ProcessTagGame(lastGame, modeConfiguration, teams, pointDeductions, separator);
        tagGameProcessor.GenerateTagSummaryData(games, modeConfiguration, teams, pointDeductions, separator);
    }

    private void GenerateTagSummaryData(IEnumerable<List<GameStats>> games,
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

    private void ProcessTagGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel,
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
}