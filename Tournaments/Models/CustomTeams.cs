using Serilog;

namespace Tournaments.Models;

public class CustomTeams
{
    public CustomTeams()
    {
        Team = new Dictionary<string, List<string>>();
    }

    public Dictionary<string, List<string>> Team { get; set; }

    public string GetPlayerTeam(string player)
    {
        var team = Team.FirstOrDefault(p => p.Value.Contains(player)).Key;

        if (string.IsNullOrEmpty(team))
        {
            Log.Warning($"No team entry for {player}");
        }

        return team;
    }
}