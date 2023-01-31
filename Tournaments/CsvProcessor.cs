using System.Diagnostics;
using System.Reflection;

namespace Tournaments;

public class CsvProcessor
{
    private readonly int _gameCount;

    public CsvProcessor(int gameCount)
    {
        _gameCount = gameCount;
    }

    public List<GameStats> ProcessCsv(GameType gameType)
    {
        var scoreList = new List<GameStats>();
        
        for (var i = 1; i <= _gameCount; i++)
        {
            var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var csvLocation = Debugger.IsAttached ?
                $"../../../Assets/{i}.csv"
                : Path.Combine(executableLocation!, $"{i}.csv");
            
            var lines = ReadFileLines(csvLocation);

            if (lines != null)
            {
                scoreList.AddRange(lines.Skip(1).Select(ProcessSoloEntry));
            }
        }

        return scoreList;
    }

    private static IEnumerable<string>? ReadFileLines(string fileName)
    {
        return File.Exists(fileName) ? File.ReadLines(fileName) : null;
    }

    private static GameStats ProcessSoloEntry(string entryLine)
    {
        var fields = entryLine.Split(',');

        var placement = int.Parse(fields[GameFieldIds.GetPlacementColumn]);
        var kills = int.Parse(fields[GameFieldIds.GetSoloKills]);
        var name = fields[GameFieldIds.GetSoloNameColumn];
        
        return new GameStats(placement, name, kills);
    }
}