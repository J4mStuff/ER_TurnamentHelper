using Serilog;
using Tournaments.Enums;
using Tournaments.Models;

namespace Tournaments.Workflow;

public class TournamentLeaderBoardCreator
{
    private readonly CsvProcessor _csvProcessor;
    private readonly ImageDrawer _imageDrawer;
    private readonly ConfigurationModel _configurationModel;

    public TournamentLeaderBoardCreator(ConfigurationModel configurationModel)
    {
        _configurationModel = configurationModel;
        _csvProcessor = new CsvProcessor(configurationModel.ColumnIds);
        _imageDrawer = new ImageDrawer();
    }

    public void GenerateData()
    {
        foreach (var modeConfiguration in _configurationModel.GetTrackedModes())
        {
            var allGames = _csvProcessor.ProcessCsv(_configurationModel.GameFiles);
            var lastGame = allGames.Last();

            switch (Enum.Parse(typeof(GameType), modeConfiguration.Name))
            {
                case GameType.Solo:
                    Log.Debug($"Processing {GameType.Solo} mode.");
                    ProcessSoloGame(lastGame, modeConfiguration);
                    GenerateSoloSummaryData(allGames, modeConfiguration);
                    break;
                case GameType.Squad:
                    Log.Debug($"Processing {GameType.Squad} mode.");
                    ProcessSquadGame(lastGame, modeConfiguration);
                    GenerateSquadSummaryData(allGames, modeConfiguration);
                    break;
                case GameType.Tag:
                    Log.Debug($"Processing {GameType.Tag} mode.");
                    var teams = CsvProcessor.ProcessTemporaryTeams();
                    ProcessTagGame(lastGame, modeConfiguration, teams);
                    GenerateTagSummaryData(allGames, modeConfiguration, teams);
                    break;
            }
        }
    }

    private void ProcessSoloGame(List<GameStats> lastGame, ModeConfiguration modeConfiguration)
    {
        lastGame.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillsMultiplier));
        lastGame = lastGame.OrderByDescending(r => r.Score).ToList();
        Log.Debug($"Got {lastGame.Count} entries for last game");
        _imageDrawer.PopulateSoloTemplate(lastGame, modeConfiguration, modeConfiguration.TemplateConfiguration.LastGameSuffix);
        Log.Information("Last game processing complete.");
    }
    
    private void ProcessSquadGame(List<GameStats> lastGame, ModeConfiguration modeConfiguration)
    {

        lastGame.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillsMultiplier));
        lastGame = lastGame.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).OrderByDescending(r => r.Score)
            .ToList();
        Log.Debug($"Got {lastGame.Count} entries for last game");
        _imageDrawer.PopulateTeamTemplate(lastGame, modeConfiguration, modeConfiguration.TemplateConfiguration.LastGameSuffix);
        Log.Information("Last game processing complete.");
    }

    private void ProcessTagGame(List<GameStats> lastGame, ModeConfiguration modeConfiguration, CustomTeams teams)
    {
        lastGame.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillsMultiplier));
        lastGame.ForEach(g => g.TeamName = teams.GetPlayerTeam(g.PlayerName));
        lastGame = lastGame.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).OrderByDescending(r => r.Score)
            .ToList();
        Log.Debug($"Got {lastGame.Count} entries for last game");
        
        _imageDrawer.PopulateTeamTemplate(lastGame, modeConfiguration, modeConfiguration.TemplateConfiguration.LastGameSuffix);
        Log.Information("Last game processing complete.");
    }

    private static GameStats ProcessSoloGroup(IGrouping<string,GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();
        Log.Debug($"Got {temp.Count} player entries for {grouping.Key}");

        var main = temp.First();
        foreach (var item in temp.Skip(1))
        {
            main.Kills += item.Kills;
            main.Score += item.Score;
        }
        Log.Debug($"Player {main.PlayerName}'s stats are processed");

        return main;
    }
    
    private GameStats ProcessTeamGroup(IGrouping<string,GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();

        if (string.IsNullOrEmpty(grouping.Key))
        {
            var message = $"Team name is blank for this team: {string.Join(",", temp.Select(x => x.PlayerName))}";
            Log.Fatal(message);
            throw new ArgumentNullException(message);
        }
        
        var main = temp.First();
        var players = new List<string>{main.PlayerName};

        foreach (var item in temp.Skip(1))
        {
            main.TeamKills += item.TeamKills;
            if (!players.Contains(item.PlayerName))
            {
                players.Add(item.PlayerName);
            }
            
            main.Kills += item.Kills;
            main.Score += item.Score;
        }

        main.PlayerName = string.Join(_configurationModel.PlayerSeparator, players);
        
        Log.Debug($"Team {grouping.Key} processed.");

        return main;
    }
    
    private void GenerateSoloSummaryData(IEnumerable<List<GameStats>> games, ModeConfiguration modeConfiguration)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillsMultiplier));

        var gameStatsList = entries.GroupBy(x => x.PlayerName).Select(ProcessSoloGroup).ToList();
        
        gameStatsList = gameStatsList.OrderByDescending(r => r.Score).ToList();
        Log.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateSoloTemplate(gameStatsList, modeConfiguration,
            modeConfiguration.TemplateConfiguration.SummarySuffix);
        Log.Information("Summary processing complete.");
    }

    private void GenerateTagSummaryData(IEnumerable<List<GameStats>> games, ModeConfiguration modeConfiguration, CustomTeams teams)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillsMultiplier));
        entries.ForEach(g => g.TeamName = teams.GetPlayerTeam(g.PlayerName));
        
        var groups = entries.GroupBy(x => x.TeamName);
        var gameStatsList = groups.Select(ProcessTeamGroup).ToList();
        
        gameStatsList = gameStatsList.OrderByDescending(r => r.Score).ToList();
        Log.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfiguration, modeConfiguration.TemplateConfiguration.SummarySuffix);
        Log.Information("Summary processing complete.");
    }

    private void GenerateSquadSummaryData(IEnumerable<List<GameStats>> games, ModeConfiguration modeConfiguration)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillsMultiplier));

        var gameStatsList = entries.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
        
        gameStatsList = gameStatsList.OrderByDescending(r => r.Score).ToList();
        Log.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfiguration,
            modeConfiguration.TemplateConfiguration.SummarySuffix);
        Log.Information("Summary processing complete.");
    }
}