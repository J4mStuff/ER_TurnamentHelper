using Tournaments.Configuration;
using Tournaments.Workflow;

namespace Tournaments;

public static class Program
{
    public static void Main(string[] args)
    {
        var manager = new ConfigManager();
        var configuration = ConfigManager.ReadAllSettings();
        
        var tournamentLeaderboardCalculator = new TournamentLeaderBoardCreator(configuration);
        tournamentLeaderboardCalculator.GenerateData();
    }
}