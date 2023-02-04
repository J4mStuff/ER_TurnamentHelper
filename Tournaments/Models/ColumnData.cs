// ReSharper disable ClassNeverInstantiated.Global
namespace Tournaments.Models;

public class ColumnData
{
    public ColumnData()
    {
        NameField = new FieldData();
        TeamNameField = new FieldData();
        KillsField = new FieldData();
        ScoreField = new FieldData();
    }
    
    public FieldData NameField { get; set; }
    public FieldData TeamNameField { get; set; }
    public FieldData KillsField { get; set; }
    public FieldData ScoreField { get; set; }
}