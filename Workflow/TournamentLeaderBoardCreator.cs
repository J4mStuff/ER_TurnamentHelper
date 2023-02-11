using Enums;
using Logger;
using Models;

namespace Workflow;

//TODO: refactor this
public class TournamentLeaderBoardCreator
{
    private readonly CsvProcessor _csvProcessor;
    private readonly ImageDrawer _imageDrawer;
    private readonly ConfigurationModel _configurationModel;
    private readonly CustomLogger _logger;

    public TournamentLeaderBoardCreator(ConfigurationModel configurationModel)
    {
        _configurationModel = configurationModel;
        _csvProcessor = new CsvProcessor(configurationModel.ColumnIds);
        _imageDrawer = new ImageDrawer();
        _logger = new CustomLogger();
    }

    public void GenerateData()
    {
        foreach (var modeConfiguration in _configurationModel.GetTrackedModes())
        {
            var allGames = _csvProcessor.ProcessCsv(_configurationModel.GameFiles);
            var lastGame = allGames.Last();
            var deductionList = _csvProcessor.ProcessPointDeductions();

            switch (Enum.Parse(typeof(GameType), modeConfiguration.Name))
            {
                case GameType.Solo:
                    _logger.Debug($"Processing {GameType.Solo} mode.");
                    ProcessSoloGame(lastGame, modeConfiguration, deductionList);
                    GenerateSoloSummaryData(allGames, modeConfiguration, deductionList);
                    break;
                case GameType.Squad:
                    _logger.Debug($"Processing {GameType.Squad} mode.");
                    ProcessSquadGame(lastGame, modeConfiguration, deductionList);
                    GenerateSquadSummaryData(allGames, modeConfiguration, deductionList);
                    break;
                case GameType.Tag:
                    _logger.Debug($"Processing {GameType.Tag} mode.");
                    var teams = _csvProcessor.ProcessTemporaryTeams();
                    ProcessTagGame(lastGame, modeConfiguration, teams, deductionList);
                    GenerateTagSummaryData(allGames, modeConfiguration, teams, deductionList);
                    break;
            }
        }
    }

    private void ProcessSoloGame(List<GameStats> gameStatsList, ModeConfiguration modeConfiguration, PointDeductions pointDeductions)
    {
        gameStatsList.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for last game");
        
        _imageDrawer.PopulateSoloTemplate(gameStatsList, modeConfiguration, modeConfiguration.TemplateConfiguration.LastGameSuffix);
        _logger.Info("Last game processing complete.");
    }
    
    private void ProcessSquadGame(List<GameStats> gameStatsList, ModeConfiguration modeConfiguration, PointDeductions pointDeductions)
    {

        gameStatsList.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for last game");
        
        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfiguration, modeConfiguration.TemplateConfiguration.LastGameSuffix);
        _logger.Info("Last game processing complete.");
    }

    private void ProcessTagGame(List<GameStats> gameStatsList, ModeConfiguration modeConfiguration, CustomTeams teams, PointDeductions pointDeductions)
    {
        gameStatsList.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        gameStatsList.ForEach(g => g.TeamName = teams.GetPlayerTeam(g.PlayerName));
        gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for last game");
        
        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfiguration, modeConfiguration.TemplateConfiguration.LastGameSuffix);
        _logger.Info("Last game processing complete.");
    }

    private GameStats ProcessSoloGroup(IGrouping<string,GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();
        _logger.Debug($"Got {temp.Count} player entries for {grouping.Key}");

        var main = temp.First();
        foreach (var item in temp.Skip(1))
        {
            main.FieldKills += item.FieldKills;
            main.ZoneKils += item.ZoneKils;
            main.Score += item.Score;
        }
        _logger.Debug($"Player {main.PlayerName}'s stats are processed");

        return main;
    }
    
    private GameStats ProcessTeamGroup(IGrouping<string,GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();

        if (string.IsNullOrEmpty(grouping.Key))
        {
            var message = $"Team name is blank for this team: {string.Join(",", temp.Select(x => x.PlayerName))}";
            _logger.Fatal(message);
            throw new ArgumentNullException(message);
        }
        var main = temp.First();

        foreach (var item in temp.Skip(1))
        {
            main.FieldKills += item.FieldKills;
            main.ZoneKils += item.ZoneKils;
            main.Score += item.Score;
        }
        
        _logger.Debug($"Team {grouping.Key} processed.");

        return main;
    }
    
    private void GenerateSoloSummaryData(IEnumerable<List<GameStats>> games, ModeConfiguration modeConfiguration, PointDeductions pointDeductions)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        var gameStatsList = entries.GroupBy(x => x.PlayerName).Select(ProcessSoloGroup).ToList();
        
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateSoloTemplate(gameStatsList, modeConfiguration,
            modeConfiguration.TemplateConfiguration.SummarySuffix);
        _logger.Info("Summary processing complete.");
    }

    private void GenerateTagSummaryData(IEnumerable<List<GameStats>> games, ModeConfiguration modeConfiguration, CustomTeams teams, PointDeductions pointDeductions)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        foreach (var entry in entries)
        {
            entry.TeamName = teams.GetPlayerTeam(entry.PlayerName);
            entry.PlayerName = string.Join(_configurationModel.PlayerSeparator,teams.GetAllTeammates(entry.TeamName));
        }

        var groups = entries.GroupBy(x => x.TeamName);
        var gameStatsList = groups.Select(ProcessTeamGroup).ToList();
        
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfiguration, modeConfiguration.TemplateConfiguration.SummarySuffix);
        _logger.Info("Summary processing complete.");
    }

    private void GenerateSquadSummaryData(IEnumerable<List<GameStats>> games, ModeConfiguration modeConfiguration, PointDeductions pointDeductions)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        var gameStatsList = entries.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();

        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfiguration,
            modeConfiguration.TemplateConfiguration.SummarySuffix);
        _logger.Info("Summary processing complete.");
    }

    private List<GameStats> SortEntries(IEnumerable<GameStats> gameStatsList)
    {
        _logger.Debug("Sorting entries.");
        return gameStatsList.OrderByDescending(r => r.Score).ThenByDescending(r => r.FieldKills).ToList();
    }
}