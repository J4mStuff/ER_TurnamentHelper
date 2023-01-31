namespace Tournaments;

public static class Program
{
    public static void Main(string[] args)
    {
        var gameCount = int.Parse(args[0]);
        var tournamentLeaderboardCalculator = new TournamentLeaderBoardCreator(gameCount);

        tournamentLeaderboardCalculator.GenerateData(GameType.Solo);
    }
}