// ReSharper disable ClassNeverInstantiated.Global
namespace Models;

public class TemplateConfigurationModel
{
    public TemplateConfigurationModel()
    {
        TemplateFileName = string.Empty;
        LastGameSuffix = string.Empty;
        SummarySuffix = string.Empty;
        NewLineDistance = 55;
        Columns = new List<ColumnData>();
    }

    public string TemplateFileName { get; set; }
    public string LastGameSuffix { get; set; }
    public string SummarySuffix { get; set; }
    public int NewLineDistance { get; set; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public List<ColumnData> Columns { get; set; }
}