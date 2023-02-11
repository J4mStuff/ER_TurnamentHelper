namespace Models;

public class GameStats
{
    public GameStats(int placement, string name,  string teamName, int zoneKils, int fieldKills)
    {
        Placements = placement;
        ZoneKils = zoneKils;
        FieldKills = fieldKills;
        PlayerName = name;
        TeamName = teamName;
    }
    
    public void CalculateScore(Dictionary<string,int> placementScoring, KillMultiplierModel killMultipliers, int deductions)
    {
        ZoneKils -= FieldKills;
        Score = 
            ZoneKils * killMultipliers.Zone
            + FieldKills * killMultipliers.Field
            + placementScoring[Placements.ToString()]
            - deductions;
    }

    public int Score { get; set; }

    public int Placements { get; }
    public string PlayerName { get; set; }
    public string TeamName { get; set; }
    public int ZoneKils { get; set; }
    public int FieldKills { get; set; }
}