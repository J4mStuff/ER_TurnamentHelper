using System.Diagnostics;
using System.Reflection;
using Tournaments.Models;

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

    public List<List<GameStats>> ProcessCsv(IEnumerable<string> fileNames)
    {
        return (from fileName in fileNames
            let executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            select Debugger.IsAttached
                ? $"../../../Stubs/{fileName}"
                : Path.Combine(executableLocation!, fileName)
            into csvLocation
            select ReadFileLines(csvLocation)
            into lines
            select lines.Skip(1).Select(ProcessEntries).ToList()).ToList();
    }
    
    public CustomTeams ProcessTemporaryTeams()
    {
        var csvLocation = Path.Combine("Assets", "teams.csv");

        var lines = ReadFileLines(csvLocation);

        var list = lines.Skip(1).Select(ProcessTeams).ToList();

        var dict = new CustomTeams();
        foreach (var item in list)
        {
            if (dict.Team.Keys.Contains(item.Key))
            {
                dict.Team[item.Key].Add(item.Value);
            }
            else
            {
                dict.Team.Add(item.Key, new List<string>{item.Value});
            }
        }

        return dict;
    }

    private static IEnumerable<string> ReadFileLines(string fileName)
    {
        if (File.Exists(fileName))
        {
            return File.ReadLines(fileName);
        }
        throw new FileNotFoundException($"File {fileName} is missing");
    }

    private KeyValuePair<string, string> ProcessTeams(string entryLine)
    {
        var fields = entryLine.Split(',');

        var nickName = fields[0];
        var teamName = fields[1];

        return new KeyValuePair<string, string>(teamName, nickName);
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