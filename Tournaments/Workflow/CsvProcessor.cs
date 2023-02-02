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

    public Dictionary<int, List<GameStats>> ProcessCsv(IEnumerable<string> fileNames)
    {
        return (from fileName in fileNames
            let executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            select Debugger.IsAttached
                ? $"../../../Stubs/{fileName}"
                : Path.Combine(executableLocation!, fileName)
            into csvLocation
            select ReadFileLines(csvLocation)).ToDictionary(lines => _gameCount, lines => lines.Skip(1).Select(ProcessEntries).ToList());
    }

    private static IEnumerable<string> ReadFileLines(string fileName)
    {
        if (File.Exists(fileName))
        {
            return File.ReadLines(fileName);
        }
        throw new FileNotFoundException($"File {fileName} is missing");
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