using Configuration;
using Workflow;

namespace Tournaments;

public static class Program
{
    public static void Main(string[] args)
    {
        var configManager = new ConfigManager();
        var configuration = configManager.ReadAllSettings();
        
        var tournamentLeaderboardCalculator = new TournamentLeaderBoardCreator(configuration);
        tournamentLeaderboardCalculator.GenerateData();
    }
}