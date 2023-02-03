namespace Tournaments.Models;

public class CustomTeams
{
    public CustomTeams()
    {
        Team = new Dictionary<string, List<string>>();
    }
    
    public Dictionary<string, List<string>> Team
    {
        get;
        set;
    }

    public string GetPlayerTeam(string player)
    {
        var teamName = Team.FirstOrDefault(p => p.Value.Contains(player)).Key;
        return teamName;
    }
}