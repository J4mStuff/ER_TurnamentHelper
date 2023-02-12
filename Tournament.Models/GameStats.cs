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

    public void UpdateScoreWithPlacement(Dictionary<string, int> placementScoring)
    {
        Score = placementScoring[Placements.ToString()];
    }

    public void CalculateSoloKillScoreWithDeductions(KillMultiplierModel killMultipliers, int deductions)
    {
        Score += ZoneKills * killMultipliers.Zone + FieldKills * killMultipliers.Field - deductions;
    }

    public void CalculateTeamKillScoreWithDeductions(KillMultiplierModel killMultipliers, int deductions)
    {
        Score += ZoneKills * killMultipliers.Zone + FieldKills * killMultipliers.Field - deductions;
    }

    public int Score { get; set; }
    public int Placements { get; init; }
    public string PlayerName { get; set; }
    public string TeamName { get; set; }
    public int ZoneKills { get; set; }
    public int FieldKills { get; set; }
    public int SoloKills { get; set; }
    public int TeamKills { get; set; }
    public List<string> PlayerList { get; set; }
}