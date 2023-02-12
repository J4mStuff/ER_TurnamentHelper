using System.Diagnostics;
using Models;

namespace Workflow.Spreadsheet;

public class GameFileSpreadsheet : SpreadsheetBase
{
    private readonly GameFieldIds _fieldIds;

    public GameFileSpreadsheet(GameFieldIds fieldIds)
    {
        _fieldIds = fieldIds;
    }

    public List<List<GameStats>> ProcessCsv(IEnumerable<string> fileNames)
    {
        return (from fileName in fileNames
            select Debugger.IsAttached
                ? $"../../../Stubs/{fileName}"
                : fileName
            into csvLocation
            select ReadCsvLines(csvLocation)
            into lines
            select lines.Skip(1).Select(ProcessEntries).ToList()).ToList();
    }
    
    private GameStats ProcessEntries(string entryLine)
    {
        var fields = entryLine.Split(',');

        var placement = int.Parse(fields[_fieldIds.PlacementColumn]);
        var name = fields[_fieldIds.PlayerNameColumn].ToUpper().Trim();
        var fieldKills = int.Parse(fields[_fieldIds.TotalFieldKills]);
        var zoneKills = int.Parse(fields[_fieldIds.SoloKillsColumn]);
        var teamName = fields.Length > _fieldIds.TeamNameColumn ? fields[_fieldIds.TeamNameColumn].ToUpper().Trim() : "N/A";
        
        Logger.Debug($"New entry: {placement}, {name}, {teamName}, {fieldKills}, {zoneKills}");

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