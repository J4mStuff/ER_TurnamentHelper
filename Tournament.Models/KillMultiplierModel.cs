namespace Models;

public class KillMultiplierModel
{
    public KillMultiplierModel()
    {
        Field = 4;
        Zone = 2;
    }
    
    public int Field { get; set; }
    public int Zone { get; set; }
}