using Models;

namespace Workflow.GameProcessors;

public class SquadGameProcessor : GameProcessorBase
{
    public void ProcessSquadGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel,
        PointDeductions pointDeductions)
    {

        //gameStatsList.ForEach(r => r.UpdateScoreWithPlacement(modeConfigurationModel.PlacementScoring,
        //    modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        //gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
        //gameStatsList = SortEntries(gameStatsList);
        //Logger.Debug($"Got {gameStatsList.Count} entries for last game");
//
        //ImageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel,
        //    modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        //Logger.Info("Last game processing complete.");
    }

    public void GenerateSquadSummaryData(IEnumerable<List<GameStats>> games,
        ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        //var entries = games.SelectMany(r => r).ToList();
        //entries.ForEach(r => r.UpdateScoreWithPlacement(modeConfigurationModel.PlacementScoring,
        //    modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
//
        //var gameStatsList = entries.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
//
        //gameStatsList = SortEntries(gameStatsList);
        //Logger.Debug($"Got {gameStatsList.Count} entries for game summary");
//
        //ImageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel,
        //    modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        //Logger.Info("Summary processing complete.");
    }
}