using Configuration;
using Enums;
using Logger;
using Models;
using Workflow.GameProcessors;
using Workflow.Spreadsheet;

namespace Workflow;

//TODO: refactor this
public class TournamentLeaderBoardCreator
{
    private readonly GameFileSpreadsheet _gameFileSpreadsheet;
    private readonly PointDeductionSpreadsheet _pointDeductionSpreadsheet;
    private readonly ConfigurationModel _configurationModel;
    private readonly CustomLogger _logger;

    public TournamentLeaderBoardCreator(ConfigurationModel configurationModel, CustomLogger logger)
    {
        _configurationModel = configurationModel;
        _gameFileSpreadsheet = new GameFileSpreadsheet(configurationModel.ColumnIds, logger);
        _pointDeductionSpreadsheet = new PointDeductionSpreadsheet(logger);
        _logger = logger;
    }

    public void GenerateData()
    {
        var configManager = new ConfigManager(_logger);
        var deductionList = _pointDeductionSpreadsheet.ProcessPointDeductions();

        foreach (var gameMode in _configurationModel.GetTrackedModes())
        {
            var modeConfiguration = configManager.ReadModeConfig(gameMode);

            var allGames = _gameFileSpreadsheet.ProcessCsv(_configurationModel.GameFiles);

            switch (Enum.Parse(typeof(GameType), modeConfiguration.Name))
            {
                case GameType.Solo:
                    var soloGameProcessor = new SoloGameProcessor(_logger);
                    soloGameProcessor.GenerateData(allGames, modeConfiguration, deductionList, _configurationModel.PlayerSeparator);
                    break;
                case GameType.Squad:
                    var squadGameProcessor = new SquadGameProcessor(_logger);
                    squadGameProcessor.GenerateData(allGames, modeConfiguration, deductionList, _configurationModel.PlayerSeparator);
                    break;
                case GameType.Tag:
                case GameType.TagDuo:
                    var tagGameProcessor = new TagGameProcessor(_logger);
                    tagGameProcessor.GenerateData(allGames, modeConfiguration, deductionList, _configurationModel.PlayerSeparator);
                    break;
            }
        }
    }
}