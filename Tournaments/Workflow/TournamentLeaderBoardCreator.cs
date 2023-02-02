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
            var results = _csvProcessor.ProcessCsv()[0]; //TODO: update this to work with multiple game files
            var modeConfiguration = _configurationModel.ModeMaps.First(m => m.Name.ToString() == type.Key);
            results.ForEach(r => r.CalculateScore(modeConfiguration.PlacementScoring, modeConfiguration.KillsMultiplier));
            results = results.OrderByDescending(r => r.Score).ToList();

            _imageDrawer.PopulateTemplate(results, modeConfiguration);
        }
    }
}