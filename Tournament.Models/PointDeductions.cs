using Logger;

namespace Models;

public class PointDeductions
{
    private readonly CustomLogger _logger;

    public PointDeductions(CustomLogger logger)
    {
        PunishmentList = new Dictionary<string, int>();
        _logger = logger;
    }

    public Dictionary<string, int> PunishmentList { get; set; }

    public int GetPlayerDeduction(string player)
    {
        var playerPunishment = PunishmentList.FirstOrDefault(p => p.Key.Contains(player)).Value;

        _logger.Debug($"Deduction for player '{player}': '{playerPunishment}'");

        return playerPunishment;
    }
}