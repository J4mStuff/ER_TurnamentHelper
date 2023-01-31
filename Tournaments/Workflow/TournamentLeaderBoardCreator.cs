using Tournaments.Configuration;

namespace Tournaments.Workflow;

public class TournamentLeaderBoardCreator
{
    private readonly CsvProcessor _csvProcessor;
    private readonly ImageDrawer _imageDrawer;
    
    public TournamentLeaderBoardCreator(ConfigurationModel model)
    {
        _csvProcessor = new CsvProcessor(model.GameCount, model.ColumnIds);
        _imageDrawer = new ImageDrawer();
    }

    public void GenerateData(Dictionary<string, bool> gameTypes)
    {
        foreach (var type in gameTypes.Where(t => t.Value))
        {
            Enum.TryParse(type.Key, out GameType typeEnum);
            GenerateGameData(typeEnum);
        }
        Console.WriteLine("Done");
    }

    private void GenerateGameData(GameType type)
    {
        var results = _csvProcessor.ProcessCsv();
        results.ForEach(r => r.CalculateScore());
        results = results.OrderByDescending(r => r.Score).ToList();

        _imageDrawer.PopulateTemplate(results);
    }
}