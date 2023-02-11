namespace Models;

public class FieldData
{
    public FieldData()
    {
        XPosition = new int();
        YPosition = new int();
        FontSize = new int();
    }

    public int XPosition { get; set; }
    public int YPosition { get; set; }
    public int FontSize { get; set; }
}