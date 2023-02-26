using Configuration;
using Logger;
using Workflow;

namespace Tournaments;

public static class Program
{
    public static void Main(string[] args)
    {
        var logger = new CustomLogger();
        var configManager = new ConfigManager(logger);
        var configuration = configManager.ReadMainConfig();

        var tournamentLeaderboardCalculator = new TournamentLeaderBoardCreator(configuration, logger);
        tournamentLeaderboardCalculator.GenerateData();
    }
}