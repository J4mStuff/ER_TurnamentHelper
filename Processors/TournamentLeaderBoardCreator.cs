using System.Data;
using System.Text.Json;
using Configuration;
using Enums;
using Logger;
using Models;
using Workflow.Spreadsheet;

namespace Workflow;

//TODO: refactor this
public class TournamentLeaderBoardCreator
{
    private readonly GameFileSpreadsheet _gameFileSpreadsheet;
    private readonly TemporaryTeamSpreadsheet _temporaryTeamSpreadsheet;
    private readonly PointDeductionSpreadsheet _pointDeductionSpreadsheet;
    private readonly ImageDrawer _imageDrawer;
    private readonly ConfigurationModel _configurationModel;
    private readonly CustomLogger _logger;

    public TournamentLeaderBoardCreator(ConfigurationModel configurationModel)
    {
        _configurationModel = configurationModel;
        _gameFileSpreadsheet = new GameFileSpreadsheet(configurationModel.ColumnIds);
        _temporaryTeamSpreadsheet = new TemporaryTeamSpreadsheet();
        _pointDeductionSpreadsheet = new PointDeductionSpreadsheet();
        _imageDrawer = new ImageDrawer();
        _logger = new CustomLogger();
    }

    public void GenerateData()
    {
        var configManager = new ConfigManager();

        foreach (var gameMode in _configurationModel.GetTrackedModes())
        {
            var modeConfiguration = configManager.ReadModeConfig(gameMode);

            var allGames = _gameFileSpreadsheet.ProcessCsv(_configurationModel.GameFiles);

            var lastGame = StupidClone(allGames.Last());
            var deductionList = _pointDeductionSpreadsheet.ProcessPointDeductions();

            switch (Enum.Parse(typeof(GameType), modeConfiguration.Name))
            {
                case GameType.Solo: //TODO fix tag style
                    _logger.Debug($"Processing {GameType.Solo} mode.");
                    ProcessSoloGame(lastGame, modeConfiguration, deductionList);
                    GenerateSoloSummaryData(allGames, modeConfiguration, deductionList);
                    break;
                case GameType.Squad: //TODO fix tag style
                    _logger.Debug($"Processing {GameType.Squad} mode.");
                    ProcessSquadGame(lastGame, modeConfiguration, deductionList);
                    GenerateSquadSummaryData(allGames, modeConfiguration, deductionList);
                    break;
                case GameType.Tag:
                    _logger.Debug($"Processing {GameType.Tag} mode.");
                    var teams = _temporaryTeamSpreadsheet.ProcessTemporaryTeams();
                    ProcessTagGame(lastGame, modeConfiguration, teams, deductionList);
                    GenerateTagSummaryData(allGames, modeConfiguration, teams, deductionList);
                    break;
            }

        }
    }

    private T StupidClone<T>(T toClone) 
    {
        var str = JsonSerializer.Serialize(toClone);
        var output = JsonSerializer.Deserialize<T>(str);

        if (output != null) return output;
        
        const string message = "Tried to clone the object which resulted in a null exception";
        _logger.Fatal(message);
        throw new NoNullAllowedException(message);
    }

    private void ProcessSoloGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        gameStatsList.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for last game");
        
        _imageDrawer.PopulateSoloTemplate(gameStatsList, modeConfigurationModel, modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        _logger.Info("Last game processing complete.");
    }
    
    private void ProcessSquadGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {

        gameStatsList.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for last game");
        
        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel, modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
        _logger.Info("Last game processing complete.");
    }

    private void ProcessTagGame(List<GameStats> gameStatsList, ModeConfigurationModel modeConfigurationModel, CustomTeams teams, PointDeductions pointDeductions)
    {
        gameStatsList.ForEach(r => r.TeamName = teams.GetPlayerTeam(r.PlayerName) ?? r.TeamName);
        gameStatsList = gameStatsList.GroupBy(x => x.TeamName).Select(ProcessLastGameTeamGroup).ToList();
        gameStatsList.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for last game");
        
        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel, modeConfigurationModel.TemplateConfiguration.LastGameSuffix);
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
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
        }
        _logger.Debug($"Player {main.PlayerName}'s stats are processed");

        return main;
    }
    
    private GameStats ProcessTeamGroup(IGrouping<string,GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();
        var playersInGame = temp.Select(p => p.PlayerName).Distinct().ToList();

        if (string.IsNullOrEmpty(grouping.Key))
        {
            var message = $"Team name is blank for this team: {string.Join(",", playersInGame)}";
            _logger.Fatal(message);
            throw new ArgumentNullException(message);
        }
        var main = temp.First();

        foreach (var item in temp.Skip(1))
        {
            main.FieldKills += item.FieldKills;
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
        }

        main.PlayerList.AddRange(playersInGame);
        main.PlayerList = main.PlayerList.Distinct().ToList();

        _logger.Debug($"Team {grouping.Key} processed.");

        return main;
    }
    
    private GameStats ProcessLastGameTeamGroup(IGrouping<string,GameStats> grouping)
    {
        var temp = grouping.Select(g => g).ToList();
        var playersInGame = temp.Select(p => p.PlayerName).Distinct().ToList();

        if (string.IsNullOrEmpty(grouping.Key))
        {
            var message = $"Team name is blank for this team: {string.Join(",", playersInGame)}";
            _logger.Fatal(message);
            throw new ArgumentNullException(message);
        }
        var main = temp.First();

        foreach (var item in temp.Skip(1))
        {
            main.FieldKills += item.FieldKills;
            main.ZoneKills += item.ZoneKills;
            main.Score += item.Score;
        }

        main.PlayerList.AddRange(playersInGame);
        main.PlayerList = main.PlayerList.Distinct().ToList();
        
        main.PlayerName = string.Join(_configurationModel.PlayerSeparator, main.PlayerList.Distinct());


        _logger.Debug($"Team {grouping.Key} processed.");

        return main;
    }
    
    private GameStats ProcessSummaryTeamGroup(IGrouping<string,GameStats> grouping)
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
        
        main.PlayerName = string.Join(_configurationModel.PlayerSeparator, main.PlayerList.Distinct());

        _logger.Debug($"Team {grouping.Key} processed.");

        return main;
    }
    
    private void GenerateSoloSummaryData(IEnumerable<List<GameStats>> games, ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        var gameStatsList = entries.GroupBy(x => x.PlayerName).Select(ProcessSoloGroup).ToList();
        
        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateSoloTemplate(gameStatsList, modeConfigurationModel,
            modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        _logger.Info("Summary processing complete.");
    }

    private void GenerateTagSummaryData(IEnumerable<List<GameStats>> games, ModeConfigurationModel modeConfigurationModel, CustomTeams teams, PointDeductions pointDeductions)
    {
        var processedGames = new List<GameStats>();
        foreach (var game in games)
        {
            game.ForEach(r => r.TeamName = teams.GetPlayerTeam(r.PlayerName) ?? r.TeamName);
            var processedGame = game.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();
            processedGame.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));
        
            processedGames.AddRange(processedGame);
        }
        
        processedGames = processedGames.GroupBy(x => x.TeamName).Select(ProcessSummaryTeamGroup).ToList();
        processedGames = SortEntries(processedGames);
        _logger.Debug($"Got {processedGames.Count} entries for game summary");

        _imageDrawer.PopulateTeamTemplate(processedGames, modeConfigurationModel, modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        _logger.Info("Summary processing complete.");
    }

    private void GenerateSquadSummaryData(IEnumerable<List<GameStats>> games, ModeConfigurationModel modeConfigurationModel, PointDeductions pointDeductions)
    {
        var entries = games.SelectMany(r => r).ToList();
        entries.ForEach(r => r.CalculateScore(modeConfigurationModel.PlacementScoring, modeConfigurationModel.KillMultiplier, pointDeductions.GetPlayerDeduction(r.PlayerName)));

        var gameStatsList = entries.GroupBy(x => x.TeamName).Select(ProcessTeamGroup).ToList();

        gameStatsList = SortEntries(gameStatsList);
        _logger.Debug($"Got {gameStatsList.Count} entries for game summary");

        _imageDrawer.PopulateTeamTemplate(gameStatsList, modeConfigurationModel, modeConfigurationModel.TemplateConfiguration.SummarySuffix);
        _logger.Info("Summary processing complete.");
    }

    private List<GameStats> SortEntries(IEnumerable<GameStats> gameStatsList)
    {
        _logger.Debug("Sorting entries.");
        return gameStatsList.OrderByDescending(r => r.Score).ThenByDescending(r => r.FieldKills).ToList();
    }
}