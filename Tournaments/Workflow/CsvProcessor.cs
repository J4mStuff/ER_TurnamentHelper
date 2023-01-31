using System.Diagnostics;
using System.Reflection;

namespace Tournaments.Workflow;

public class CsvProcessor
{
    private readonly int _gameCount;
    private readonly GameFieldIds _fieldIds;

    public CsvProcessor(int gameCount, GameFieldIds fieldIds)
    {
        _fieldIds = fieldIds;
        _gameCount = gameCount;
    }

    public List<GameStats> ProcessCsv()
    {
        var scoreList = new List<GameStats>();

        for (var i = 1; i <= _gameCount; i++)
        {
            var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var csvLocation = Debugger.IsAttached
                ? $"../../../Stubs/{i}.csv"
                : Path.Combine(executableLocation!, $"{i}.csv");

            var lines = ReadFileLines(csvLocation);

            if (lines != null)
            {
                scoreList.AddRange(lines.Skip(1).Select(ProcessEntries));
            }
        }

        return scoreList;
    }

    private static IEnumerable<string>? ReadFileLines(string fileName)
    {
        return File.Exists(fileName) ? File.ReadLines(fileName) : null;
    }

    private GameStats ProcessEntries(string entryLine)
    {
        var fields = entryLine.Split(',');

        var placement = int.Parse(fields[_fieldIds.PlacementColumn]);
        var name = fields[_fieldIds.PlayerNameColumn];
        var kills = int.Parse(fields[_fieldIds.SoloKillsColumn]);
        var teamKills = int.Parse(fields[_fieldIds.TeamKillsColumn]);
        var teamName = fields.Length >= _fieldIds.TeamNameColumn ? fields[_fieldIds.TeamNameColumn] : "N/A";

        return new GameStats(placement, name, teamName, kills, teamKills);
    }
}