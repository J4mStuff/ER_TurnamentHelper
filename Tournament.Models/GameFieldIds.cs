namespace Models;

public class GameFieldIds
{
    public GameFieldIds()
    {
        PlacementColumn = new int();
        PlayerNameColumn = new int();
        TotalFieldKills = new int();
        SoloKillsColumn = new int();
        TeamKillsColumn = new int();
        TeamNameColumn = new int();
    }

    public int PlacementColumn { get; set; }
    public int PlayerNameColumn { get; set; }
    public int TotalFieldKills { get; set; }
    public int TeamKillsColumn { get; set; }
    public int SoloKillsColumn { get; set; }
    public int TeamNameColumn { get; set; }
}