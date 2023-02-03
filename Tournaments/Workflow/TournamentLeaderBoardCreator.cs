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
        _csvProcessor = new CsvProcessor(configurationModel.GameFiles.Count, configurationModel.ColumnIds);
        _imageDrawer = new ImageDrawer();
    }

    public void GenerateData(Dictionary<string, bool> gameTypes)
    {
        foreach (var type in gameTypes.Where(t => t.Value))
        {
            var allGames = _csvProcessor.ProcessCsv(_configurationModel.GameFiles);
            var lastGame = allGames.First();
            GenerateLastGameData(lastGame, type.Key);
            GenerateSummaryData(allGames, type.Key);
        }
    }

    public void GenerateLastGameData(List<GameStats> game, string type)
    {
        var modeConfiguration = _configurationModel.ModeMaps.First(m => m.Name.ToString() == type);
        game.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.killsMultiplier));
        game = game.OrderByDescending(r => r.Score).ToList();
        
        _imageDrawer.PopulateTemplate(game, modeConfiguration, modeConfiguration.TemplateConfiguration.LastGameSuffix);
    }

    public void GenerateSummaryData(List<List<GameStats>> games, string type)
    {
        var modeConfiguration = _configurationModel.ModeMaps.First(m => m.Name.ToString() == type);

        List<GameStats> gameStatsList = games.First();

        foreach (var player in gameStatsList)
        {
            GameStats playerStats;
            
            foreach (var game in games.Skip(1))
            {
                playerStats = game.First(p => p.PlayerName == player.PlayerName);
                player.Kills += playerStats.Kills;
                player.TeamKills += playerStats.TeamKills;
            }
        }
        
        gameStatsList.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.killsMultiplier));
        gameStatsList = gameStatsList.OrderByDescending(r => r.Score).ToList();

        _imageDrawer.PopulateTemplate(gameStatsList, modeConfiguration, modeConfiguration.TemplateConfiguration.SummarySuffix);
    }
}