namespace Tournaments.Models;

public class TemplateModeSettings
{
    public string TemplateFileName { get; set; }
    public string LastGameSuffix { get; set; }
    public string SummarySuffix { get; set; }
    public int YStartingPosition { get; set; }
    public int NewLineDistance { get; set; }
    public List<ColumnData> Columns { get; set; }
}