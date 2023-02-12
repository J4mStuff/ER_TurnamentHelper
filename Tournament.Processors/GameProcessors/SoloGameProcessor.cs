using Models;

namespace Workflow.GameProcessors;

public class SoloGameProcessor : GameProcessorBase
{
    public void ProcessSoloGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel,
        PointDeductions pointDeductions)
    {
        gameStatsList.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring,
            modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        gameStatsList = SortEntries(gameStatsList);
        Logger.Debug($"Got {gameStatsList.Count} entries for last game");

        ImageDrawer.PopulateSoloTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        Logger.Info("Last game processing complete.");
    }

    public void GenerateSoloSummaryData(IEnumerable<List<GameStats>> games,
        ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring,
            modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        var gameStatsList = entries.GroupBy(x => x.PlayerName).Select(ProcessSoloGroup).ToList();

        gameStatsList = SortEntries(gameStatsList);
        Logger.Debug($"Got {gameStatsList.Count} entries for game summary");

        ImageDrawer.PopulateSoloTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        Logger.Info("Summary processing complete.");
    }
}