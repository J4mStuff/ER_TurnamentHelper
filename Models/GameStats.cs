namespace Models;

public class GameStats
{
    public GameStats(int placement, string name,  string teamName, int kills, int teamKills)
    {
        Placements = placement;
        Kills = kills;
        TeamKills = teamKills;
        PlayerName = name;
        TeamName = teamName;
    }
    
    public void CalculateScore(Dictionary<string,int> placementScoring, int killsMultiplier)
    {
        Score = Kills * killsMultiplier + placementScoring[Placements.ToString()];
    }

    public int Score { get; set; }

    public int Placements { get; }

    public int Kills { get; set; }

    public string PlayerName { get; set; }
    public string TeamName { get; set; }
    public int TeamKills { get; set; }
}