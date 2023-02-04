using System.Diagnostics;
using Serilog;
using Serilog.Events;
using Tournaments.Configuration;
using Tournaments.Workflow;

namespace Tournaments;

public static class Program
{
    public static void Main(string[] args)
    {
        var consoleRestriction = Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Warning;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: consoleRestriction)
            .WriteTo.File(Path.Combine("logs", "main.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        var configuration = ConfigManager.ReadAllSettings();
        
        var tournamentLeaderboardCalculator = new TournamentLeaderBoardCreator(configuration);
        tournamentLeaderboardCalculator.GenerateData();
    }
}