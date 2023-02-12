using Models;

namespace Workflow.Spreadsheet;

public class TemporaryTeamSpreadsheet : SpreadsheetBase
{
    public CustomTeams ProcessTemporaryTeams()
    {
        var csvLocation = Path.Combine("assets", "teams.csv");

        var lines = ReadCsvLines(csvLocation);
        var list = lines.Skip(1).Select(ProcessTeams).ToList();
        
        Logger.Info($"Teams parsed.");

        var dict = new CustomTeams();
        foreach (var item in list)
        {
            if (dict.Team.Keys.Contains(item.Key))
            {
                dict.Team[item.Key].Add(item.Value);
                Logger.Debug($"Updated {item.Key} with {item.Value}");
            }
            else
            {
                dict.Team.Add(item.Key, new List<string>{item.Value});
                Logger.Debug($"New {item.Key} - {item.Value}");
            }
        }

        Logger.Info($"Teams built.");

        return dict;
    }
    
    private KeyValuePair<string, string> ProcessTeams(string entryLine)
    {
        var fields = entryLine.Split(',');

        var nickName = fields[0].ToUpper().Trim();
        var teamName = fields[1].ToUpper().Trim();
        
        Logger.Debug($"Got player '{nickName}', team: '{teamName}'");

        return new KeyValuePair<string, string>(teamName, nickName);
    }
}