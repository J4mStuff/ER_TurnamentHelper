namespace Models;

public class GameStats
{
    public void CalculateScore(Dictionary<string,int> placementScoring, KillMultiplierModel killMultipliers, int deductions)
    {
        Score = 
            ZoneKils * killMultipliers.Zone
            + FieldKills * killMultipliers.Field
            + placementScoring[Placements.ToString()]
            - deductions;
    }

    public int Score { get; set; }
    public int Placements { get; set; }
    public string PlayerName { get; set; }
    public string TeamName { get; set; }
    public int ZoneKils { get; set; }
    public int FieldKills { get; set; }
}