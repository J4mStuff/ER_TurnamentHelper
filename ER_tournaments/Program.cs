using ER_tournaments;


var gameCount = int.Parse(args[0]);
var tournamentLeaderboardCalculator = new TournamentLeaderBoardCreator(gameCount);

tournamentLeaderboardCalculator.GenerateData(GameType.Solo);