namespace Tournaments;

public class GameStats
{
    private readonly Dictionary<int, int> _placementPoints = new()
    {
        { 1, 36 },
        { 2, 26 },
        { 3, 20 },
        { 4, 16 },
        { 5, 10 },
        { 6, 4 }
    };
    
    public GameStats(int placement, string name,  int kills)
    {
        Placements = placement;
        Kills = kills;
        Name = name;
    }
    
    public void CalculateScore()
    {
        Score = Kills * 4 + _placementPoints[Placements];
    }

    public int Score { get; set; }

    public int Placements { get; }

    public int Kills { get; }

    public string Name { get; }
}