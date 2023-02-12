using Enums;
using Logger;
using Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Workflow;

public class ImageDrawer
{
    private FontFamily _customFonts;
    private const string OutputDirectory = "leaderboards";
    private readonly CustomLogger _logger;

    public ImageDrawer()
    {
        _logger = new CustomLogger();
    }

    public void PopulateSoloTemplate(List<GameStats> statsList, ModeConfigurationModel config, string suffix)
    {
        _logger.Debug("Populating Solo Template");
        var fonts = new FontCollection();
        _customFonts = fonts.Add(Path.Combine("assets", $"default_uwu.ttf"));
        Directory.CreateDirectory(OutputDirectory);
        DrawStats(config, statsList, $"{config.Name}{suffix}", false);
    }

    public void PopulateTeamTemplate(List<GameStats> statsList, ModeConfigurationModel config, string suffix)
    {
        _logger.Debug("Populating Solo Template");
        var fonts = new FontCollection();
        _customFonts = fonts.Add(Path.Combine("assets", config.FontName));
        Directory.CreateDirectory(OutputDirectory);
        DrawStats(config, statsList, $"{config.Name}{suffix}", true);
    }


    private void DrawStats(ModeConfigurationModel config, IReadOnlyCollection<GameStats> statsList, string outFileName,
        bool teamMode)
    {
        var image = Image.Load(Path.Combine("assets", config.TemplateConfiguration.TemplateFileName));
        _logger.Debug($"Loading image: {config.TemplateConfiguration.TemplateFileName}");

        var columns = config.TemplateConfiguration.Columns.Count;
        var entriesPerColum = statsList.Count / columns;
        _logger.Debug($"Creating {columns} columns with {entriesPerColum} players each");

        var processed = 0;
        foreach (var column in config.TemplateConfiguration.Columns)
        {
            var chunk = statsList.Skip(processed).Take(entriesPerColum).ToList();
            _logger.Debug($"Processing next chunk of players: {string.Join(",", chunk.Select(c => c.PlayerName))}");
            image = MutateImage(image, config, column, chunk, teamMode);
            processed += chunk.Count;
        }

        var outputImagePath = $"{OutputDirectory}/{outFileName}.png";
        image.Save(outputImagePath);
        _logger.Info($"Processing of image {outputImagePath} completed.");
    }

    private Image? MutateImage(Image? image, ModeConfigurationModel config, ColumnData columnData,
        List<GameStats> statsList, bool teamMode)
    {
        var colour = Color.FromRgb(config.FontColour[ColourCodes.R], config.FontColour[ColourCodes.G],
            config.FontColour[ColourCodes.B]);
        var multiplier = 1;

        foreach (var stats in statsList)
        {
            var multi = multiplier * config.TemplateConfiguration.NewLineDistance;

            if (teamMode)
            {
                image = MutateImage(image, stats.TeamName,
                    _customFonts.CreateFont(columnData.TeamNameField.FontSize), colour,
                    new PointF(columnData.TeamNameField.XPosition, columnData.TeamNameField.YPosition + multi));
                image = MutateImage(image, stats.PlayerName,
                    // ReSharper disable once PossibleLossOfFraction
                    _customFonts.CreateFont(columnData.NameField.FontSize), colour,
                    new PointF(columnData.NameField.XPosition, columnData.NameField.YPosition + multi));
            }
            else
            {
                image = MutateImage(image, stats.PlayerName,
                    _customFonts.CreateFont(columnData.NameField.FontSize), colour,
                    new PointF(columnData.NameField.XPosition, columnData.NameField.YPosition + multi));
            }

            image = MutateImage(image, (stats.TeamKills).ToString(),
                _customFonts.CreateFont(columnData.KillsField.FontSize), colour,
                new PointF(columnData.KillsField.XPosition, columnData.KillsField.YPosition + multi));
            image = MutateImage(image, stats.Score.ToString(),
                _customFonts.CreateFont(columnData.ScoreField.FontSize), colour,
                new PointF(columnData.ScoreField.XPosition, columnData.ScoreField.YPosition + multi));
            multiplier++;
            _logger.Debug($"Entry completed.");

        }

        return image;
    }

    private Image? MutateImage(Image? image, string? text, Font font, Color colour, PointF position)
    {
        _logger.Debug($"Drawing entry: {text}");
        image.Mutate(op => op.DrawText(text, font, colour, position));
        return image;
    }
}