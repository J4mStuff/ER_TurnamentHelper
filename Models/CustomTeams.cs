using Logger;

namespace Models;

public class CustomTeams
{
    private readonly CustomLogger _logger;

    public CustomTeams()
    {
        Team = new Dictionary<string, List<string>>();
        _logger = new CustomLogger();
    }

    public Dictionary<string, List<string>> Team { get; set; }

    public string GetPlayerTeam(string player)
    {
        var team = Team.FirstOrDefault(p => p.Value.Contains(player)).Key;

        if (string.IsNullOrEmpty(team))
        {
            _logger.Warning($"No team entry for {player}");
        }

        return team;
    }
    
    public IEnumerable<string> GetAllTeammates(string team)
    {
        var players = Team.FirstOrDefault(p => p.Key == team).Value;

        if (string.IsNullOrEmpty(team))
        {
            _logger.Warning($"Team {team} doesn't exist.");
        }

        return players;
    }
}