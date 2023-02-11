using System.Collections.Immutable;
using System.Diagnostics;
using Logger;
using Models;

namespace Workflow;

//TODO refactor this
public class CsvProcessor
{
    private readonly GameFieldIds _fieldIds;
    private readonly CustomLogger _logger;

    public CsvProcessor(GameFieldIds fieldIds)
    {
        _fieldIds = fieldIds;
        _logger = new CustomLogger();
    }

    public List<List<GameStats>> ProcessCsv(IEnumerable<string> fileNames)
    {
        return (from fileName in fileNames
            //let executableLocation = Directory.GetCurrentDirectory()
            select Debugger.IsAttached
                ? $"../../../Stubs/{fileName}"
                : fileName
            into csvLocation
            select ReadFileLines(csvLocation)
            into lines
            select lines.Skip(1).Select(ProcessEntries).ToList()).ToList();
    }
    
    public CustomTeams ProcessTemporaryTeams()
    {
        var csvLocation = Path.Combine("assets", "teams.csv");

        var lines = ReadFileLines(csvLocation);
        var list = lines.Skip(1).Select(ProcessTeams).ToList();
        
        _logger.Info($"Teams parsed.");

        var dict = new CustomTeams();
        foreach (var item in list)
        {
            if (dict.Team.Keys.Contains(item.Key))
            {
                dict.Team[item.Key].Add(item.Value);
                _logger.Debug($"Updated {item.Key} with {item.Value}");
            }
            else
            {
                dict.Team.Add(item.Key, new List<string>{item.Value});
                _logger.Debug($"New {item.Key} - {item.Value}");
            }
        }

        _logger.Info($"Teams built.");

        return dict;
    }
    
    public PointDeductions ProcessPointDeductions()
    {
        var csvLocation = Path.Combine("assets", "playerDeductions.csv");

        var lines = ReadFileLines(csvLocation);
        var list = lines.Skip(1).Select(ProcessDeductions).ToList();
        
        _logger.Info($"Deductions parsed.");

        var dict = new PointDeductions();
        foreach (var item in list)
        {
            if (dict.PunishmentList.Keys.Contains(item.Key))
            {
                dict.PunishmentList[item.Key] += item.Value;
                _logger.Debug($"Updated {item.Key} with {item.Value}");
            }
            else
            {
                dict.PunishmentList[item.Key] = item.Value;
                _logger.Debug($"New {item.Key} - {item.Value}");
            }
        }

        _logger.Info($"Deductions built.");

        return dict;
    }

    private IEnumerable<string> ReadFileLines(string fileName)
    {
        if (File.Exists(fileName))
        {
            return File.ReadLines(fileName);
        }
        _logger.Fatal($"File {fileName} is missing");
        return ImmutableArray<string>.Empty;
    }

    private KeyValuePair<string, string> ProcessTeams(string entryLine)
    {
        var fields = entryLine.Split(',');

        var nickName = fields[0].ToUpper().Trim();
        var teamName = fields[1].ToUpper().Trim();
        
        _logger.Debug($"Got player '{nickName}', team: '{teamName}'");

        return new KeyValuePair<string, string>(teamName, nickName);
    }
    
    private KeyValuePair<string, int> ProcessDeductions(string entryLine)
    {
        var fields = entryLine.Split(',');

        var player = fields[0].ToUpper().Trim();
        var deduction = int.Parse(fields[1].ToUpper().Trim());
        
        _logger.Debug($"Got player '{player}', team: '{deduction}'");

        return new KeyValuePair<string, int>(player, deduction);
    }
    
    private GameStats ProcessEntries(string entryLine)
    {
        var fields = entryLine.Split(',');

        var placement = int.Parse(fields[_fieldIds.PlacementColumn]);
        var name = fields[_fieldIds.PlayerNameColumn].ToUpper().Trim();
        var fieldKills = int.Parse(fields[_fieldIds.TotalFieldKills]);
        var zoneKills = int.Parse(fields[_fieldIds.SoloKillsColumn]) - fieldKills;
        var teamName = fields.Length > _fieldIds.TeamNameColumn ? fields[_fieldIds.TeamNameColumn].ToUpper().Trim() : "N/A";
        
        _logger.Debug($"New entry: {placement}, {name}, {teamName}, {fieldKills}, {zoneKills}");

        return new GameStats
        {
            FieldKills = fieldKills,
            PlayerName = name,
            TeamName = teamName,
            ZoneKills = zoneKills,
            Placements = placement
        };
    }
}