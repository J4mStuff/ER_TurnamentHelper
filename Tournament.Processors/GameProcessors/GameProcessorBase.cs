using Helpers;
using Logger;
using Models;

namespace Workflow.GameProcessors;

public class GameProcessorBase
{
    protected readonly CustomLogger Logger;
    protected readonly ImageDrawer ImageDrawer;
    protected readonly StupidClone StupidClone;

    protected GameProcessorBase()
    {
        Logger = new CustomLogger();
        ImageDrawer = new ImageDrawer();
        StupidClone = new StupidClone();
    }

    protected List<GameStats> SortEntries(IEnumerable<GameStats> gameStatsList)
    {
        Logger.Debug("Sorting entries.");
        return gameStatsList.OrderByDescending(r => r.Score).ThenByDescending(r => r.FieldKills).ToList();
    }

    protected GameStats ProcessTeamGroup(IGrouping<string, GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();
        var playersInGame = temp.Select(p => p.PlayerName).Distinct().ToList();

        if (string.IsNullOrEmpty(grouping.Key))
        {
            var message = $"Team name is blank for this team: {string.Join(",", playersInGame)}";
            Logger.Fatal(message);
            throw new ArgumentNullException(message);
        }

        var main = temp.First();

        foreach (var item in temp.Skip(1))
        {
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
        }

        main.PlayerList.AddRange(playersInGame);
        main.PlayerList = main.PlayerList.Distinct().ToList();

        Logger.Debug($"Team {grouping.Key} processed.");

        return main;
    }

    private GameStats ProcessSingleGameTeamGroup(IGrouping<string, GameStats> grouping, string separator)
    {
        var temp = grouping.Select(g => g).ToList();
        var playersInGame = temp.Select(p => p.PlayerName).Distinct().ToList();

        if (string.IsNullOrEmpty(grouping.Key))
        {
            var message = $"Team name is blank for this team: {string.Join(",", playersInGame)}";
            Logger.Fatal(message);
            throw new ArgumentNullException(message);
        }

        var main = temp.First();

        foreach (var item in temp.Skip(1))
        {
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
        }

        main.PlayerList.AddRange(playersInGame);
        main.PlayerList = main.PlayerList.Distinct().ToList();

        main.PlayerName = string.Join(separator, main.PlayerList.Distinct());


        Logger.Debug($"Team {grouping.Key} processed.");

        return main;
    }

    protected GameStats ProcessSummaryTeamGroup(IGrouping<string, GameStats> grouping, string separator)
    {
        var temp = grouping.Select(g => g).ToList();

        var main = temp.First();

        foreach (var item in temp.Skip(1))
        {
            main.FieldKills += item.FieldKills;
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
            main.PlayerList.AddRange(item.PlayerList);
        }

        main.PlayerName = string.Join(separator, main.PlayerList.Distinct());

        Logger.Debug($"Team {grouping.Key} processed.");

        return main;
    }

    protected GameStats ProcessSoloGroup(IGrouping<string, GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();
        Logger.Debug($"Got {temp.Count} player entries for {grouping.Key}");

        var main = temp.First();
        foreach (var item in temp.Skip(1))
        {
            //main.FieldKills += item.FieldKills;
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
        }

        Logger.Debug($"Player {main.PlayerName}'s stats are processed");

        return main;
    }

    protected List<GameStats> ProcessSingleTagGame(List<GameStats> gameStatsList, CustomTeams teams,
        ModeConfigurationModel modeConfigurationModel, string separator)
    {
        gameStatsList.ForEach(r => r.TeamName = teams.GetPlayerTeam(r.PlayerName) ?? r.TeamName);
        return ProcessSingleTeamGame(gameStatsList, modeConfigurationModel, separator);
    }

    protected List<GameStats> ProcessSingleTeamGame(List<GameStats> gameStatsList,
        ModeConfigurationModel modeConfigurationModel, string separator)
    {
        gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(g
            => ProcessSingleGameTeamGroup(g, separator)).ToList();
        return ProcessSingleGame(gameStatsList, modeConfigurationModel);
    }

    protected List<GameStats> ProcessSingleGame(List<GameStats> gameStatsList,
        ModeConfigurationModel modeConfigurationModel)
    {
        gameStatsList.ForEach(r => r.UpdateScoreWithPlacement(modeConfigurationModel.PlacementScoring));
        Logger.Debug($"Got {gameStatsList.Count} entries for last game");

        return gameStatsList;
    }
}