using Configuration;
using Enums;
using Helpers;
using Logger;
using Models;
using Workflow.GameProcessors;
using Workflow.Spreadsheet;

namespace Workflow;

//TODO: refactor this
public class TournamentLeaderBoardCreator
{
    private readonly GameFileSpreadsheet _gameFileSpreadsheet;
    private readonly TemporaryTeamSpreadsheet _temporaryTeamSpreadsheet;
    private readonly PointDeductionSpreadsheet _pointDeductionSpreadsheet;
    private readonly ConfigurationModel _configurationModel;
    private readonly CustomLogger _logger;
    private readonly StupidClone _stupidClone;

    public TournamentLeaderBoardCreator(ConfigurationModel configurationModel)
    {
        _configurationModel = configurationModel;
        _gameFileSpreadsheet = new GameFileSpreadsheet(configurationModel.ColumnIds);
        _temporaryTeamSpreadsheet = new TemporaryTeamSpreadsheet();
        _pointDeductionSpreadsheet = new PointDeductionSpreadsheet();
        _logger = new CustomLogger();
        _stupidClone = new StupidClone();
    }

    public void GenerateData()
    {
        var configManager = new ConfigManager();

        foreach (var gameMode in _configurationModel.GetTrackedModes())
        {
            var modeConfiguration = configManager.ReadModeConfig(gameMode);

            var allGames = _gameFileSpreadsheet.ProcessCsv(_configurationModel.GameFiles);

            var lastGame = _stupidClone.PerformStupidClone(allGames.Last());
            var deductionList = _pointDeductionSpreadsheet.ProcessPointDeductions();

            switch (Enum.Parse(typeof(GameType), modeConfiguration.Name))
            {
                case GameType.Solo: //TODO fix tag style
                    _logger.Debug($"Processing {modeConfiguration.Name} mode.");
                    var soloGameProcessor = new SoloGameProcessor();
                    soloGameProcessor.ProcessSoloGame(lastGame, modeConfiguration, deductionList);
                    soloGameProcessor.GenerateSoloSummaryData(allGames, modeConfiguration, deductionList);
                    break;
                case GameType.Squad: //TODO fix tag style
                    _logger.Debug($"Processing {modeConfiguration.Name} mode.");
                    var squadGameProcessor = new SquadGameProcessor();
                    squadGameProcessor.ProcessSquadGame(lastGame, modeConfiguration, deductionList);
                    squadGameProcessor.GenerateSquadSummaryData(allGames, modeConfiguration, deductionList);
                    break;
                case GameType.Tag:
                case GameType.TagDuo:
                    _logger.Debug($"Processing {modeConfiguration.Name} mode.");
                    var teams = _temporaryTeamSpreadsheet.ProcessTemporaryTeams();
                    var tagGameProcessor = new TagGameProcessor();
                    tagGameProcessor.ProcessTagGame(lastGame, modeConfiguration, teams, deductionList,
                        _configurationModel.PlayerSeparator);
                    tagGameProcessor.GenerateTagSummaryData(allGames, modeConfiguration, teams, deductionList,
                        _configurationModel.PlayerSeparator);
                    break;
            }

        }
    }
}