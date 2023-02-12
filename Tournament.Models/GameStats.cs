namespace Models;

public class GameStats
{
    public GameStats()
    {
        Score = new int();
        Placements = new int();
        PlayerName = string.Empty;
        TeamName = string.Empty;
        ZoneKills = new int();
        FieldKills = new int();
        PlayerList = new List<string>();
    }

    public void CalculateScore(Dictionary<string, int> placementScoring, KillMultiplierModel killMultipliers,
        int deductions)
    {
        Score =
            ZoneKills * killMultipliers.Zone
            + FieldKills * killMultipliers.Field
            + placementScoring[Placements.ToString()]
            - deductions;
    }

    public int Score { get; set; }
    public int Placements { get; init; }
    public string PlayerName { get; set; }
    public string TeamName { get; set; }
    public int ZoneKills { get; set; }
    public int FieldKills { get; set; }
    public List<string> PlayerList { get; set; }
}