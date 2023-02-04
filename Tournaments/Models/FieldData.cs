namespace Tournaments.Models;

public class FieldData
{
    public FieldData()
    {
        XPosition = 0;
        YPosition = 0;
        FontSize = 0;
    }

    public int XPosition { get; set; }
    public int YPosition { get; set; }
    public int FontSize { get; set; }
}