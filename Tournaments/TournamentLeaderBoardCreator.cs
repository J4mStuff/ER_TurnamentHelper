namespace Tournaments;

public class TournamentLeaderBoardCreator
{
    private readonly CsvProcessor _csvProcessor;
    private readonly ImageDrawer _imageDrawer;
    
    public TournamentLeaderBoardCreator(int gameCount)
    {
        _csvProcessor = new CsvProcessor(gameCount);
        _imageDrawer = new ImageDrawer();
    }

    public void GenerateData(GameType gameType)
    {
        var results = _csvProcessor.ProcessCsv(gameType);
        results.ForEach(r => r.CalculateScore());
        results = results.OrderByDescending(r => r.Score).ToList();

        _imageDrawer.PopulateTemplate(results);
        
        Console.WriteLine("Done");
    }
}