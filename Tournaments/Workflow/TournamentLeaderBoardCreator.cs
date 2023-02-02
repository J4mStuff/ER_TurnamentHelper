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
            var lastGame = allGames.First().Value; //TODO make it work for all games
            var modeConfiguration = _configurationModel.ModeMaps.First(m => m.Name.ToString() == type.Key);
            lastGame.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.killsMultiplier));
            lastGame = lastGame.OrderByDescending(r => r.Score).ToList();

            _imageDrawer.PopulateTemplate(lastGame, modeConfiguration);
        }
    }
}