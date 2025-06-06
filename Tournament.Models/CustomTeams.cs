using Logger;

namespace Models;

public class CustomTeams
{
    private readonly CustomLogger _logger;

    public CustomTeams(CustomLogger logger)
    {
        Team = new Dictionary<string, List<string>>();
        _logger = logger;
    }

    public Dictionary<string, List<string>> Team { get; set; }

    public string? GetPlayerTeam(string player)
    {
        var team = Team.FirstOrDefault(p => p.Value.Contains(player)).Key;

        if (!string.IsNullOrEmpty(team)) return team;

        _logger.Warning($"No team entry for {player}");
        return null;
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