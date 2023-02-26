using System.Collections.Immutable;
using Logger;

namespace Workflow.Spreadsheet;

public class SpreadsheetBase
{
    protected readonly CustomLogger Logger;

    protected SpreadsheetBase(CustomLogger logger)
    {
        Logger = logger;
    }

    protected IEnumerable<string> ReadCsvLines(string fileName)
    {
        try
        {
            return File.ReadLines(fileName);
        }
        catch (Exception ex)
        {
            Logger.Fatal($"Reading file {fileName} failed with exception: {ex.Message}");
            throw;
        }
    }
}