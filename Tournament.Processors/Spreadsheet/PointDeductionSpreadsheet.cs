using Logger;
using Models;

namespace Workflow.Spreadsheet;

public class PointDeductionSpreadsheet : SpreadsheetBase
{
    public PointDeductionSpreadsheet(CustomLogger logger) : base(logger)
    {
    }
    
    public PointDeductions ProcessPointDeductions()
    {
        var csvLocation = Path.Combine("assets", "playerDeductions.csv");

        var lines = ReadCsvLines(csvLocation);
        var list = lines.Skip(1).Select(ProcessDeductions).ToList();
        
        Logger.Info($"Deductions parsed.");

        var dict = new PointDeductions(Logger);
        foreach (var item in list)
        {
            if (dict.PunishmentList.Keys.Contains(item.Key))
            {
                dict.PunishmentList[item.Key] += item.Value;
                Logger.Debug($"Updated {item.Key} with {item.Value}");
            }
            else
            {
                dict.PunishmentList[item.Key] = item.Value;
                Logger.Debug($"New {item.Key} - {item.Value}");
            }
        }

        Logger.Info($"Deductions built.");

        return dict;
    }
    
    private KeyValuePair<string, int> ProcessDeductions(string entryLine)
    {
        var fields = entryLine.Split(',');

        var player = fields[0].ToUpper().Trim();
        var deduction = int.Parse(fields[1].ToUpper().Trim());
        
        Logger.Debug($"Got player '{player}', team: '{deduction}'");

        return new KeyValuePair<string, int>(player, deduction);
    }
}