using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using Tournaments.Enums;
using Tournaments.Models;

namespace Tournaments.Workflow;

public class ImageDrawer
{
    private FontFamily _customFonts;
    private const string OutputDirectory = "leaderboards";

    public void PopulateSoloTemplate(List<GameStats> statsList, ModeConfiguration config, string suffix)
    {
        var fonts = new FontCollection();
        _customFonts = fonts.Add(Path.Combine("Assets", $"default_uwu.ttf"));
        Directory.CreateDirectory(OutputDirectory);
        DrawStats(config, statsList, $"{config.Name}{suffix}", false);
    }
    
    public void PopulateTeamTemplate(List<GameStats> statsList, ModeConfiguration config, string suffix)
    {
        var fonts = new FontCollection();
        _customFonts = fonts.Add(Path.Combine("Assets", $"default_uwu.ttf"));
        Directory.CreateDirectory(OutputDirectory);
        DrawStats(config, statsList, $"{config.Name}{suffix}", true);
    }


    private void DrawStats(ModeConfiguration config, IReadOnlyCollection<GameStats> statsList, string outFileName, bool teamMode)
    {
        var image = Image.Load(Path.Combine("Assets", config.TemplateConfiguration.TemplateFileName));

        var columns = config.TemplateConfiguration.Columns.Count;
        var entriesPerColum = statsList.Count / columns;
        
        var processed = 0;
        foreach (var column in config.TemplateConfiguration.Columns)
        {
            var chunk = statsList.Skip(processed).Take(entriesPerColum).ToList();
            image = MutateImage(image, config, column, chunk, teamMode);
            processed += chunk.Count;
        }
        
        image.Save($"{OutputDirectory}/{outFileName}.png");
    }

    private Image? MutateImage(Image? image, ModeConfiguration config, ColumnData columnData, List<GameStats> statsList, bool teamMode)
    {
        var colour = Color.FromRgb(config.FontColour[ColourCodes.R], config.FontColour[ColourCodes.G], config.FontColour[ColourCodes.B]);
        var multiplier = 1;

        foreach (var stats in statsList)
        {
            var multi = multiplier * config.TemplateConfiguration.NewLineDistance;

            if (teamMode)
            {
                image = MutateImage(image, stats.TeamName,
                    _customFonts.CreateFont(columnData.NameField.FontSize), colour,
                    new PointF(columnData.NameField.XPosition, columnData.TeamNameField.YPosition+multi));
                image = MutateImage(image, stats.PlayerName,
                    // ReSharper disable once PossibleLossOfFraction
                    _customFonts.CreateFont(columnData.NameField.FontSize/2), colour,
                    new PointF(columnData.NameField.XPosition, columnData.NameField.YPosition+multi));
            }
            else
            {
                image = MutateImage(image, stats.PlayerName,
                    _customFonts.CreateFont(columnData.NameField.FontSize), colour,
                    new PointF(columnData.NameField.XPosition, columnData.NameField.YPosition+multi));
            }
            image = MutateImage(image, stats.Kills.ToString(),
                _customFonts.CreateFont(columnData.KillsField.FontSize), colour,
                new PointF(columnData.KillsField.XPosition, columnData.KillsField.YPosition+multi));
            image = MutateImage(image, stats.Score.ToString(),
                _customFonts.CreateFont(columnData.ScoreField.FontSize), colour,
                new PointF(columnData.ScoreField.XPosition, columnData.ScoreField.YPosition+multi));
            multiplier++;
        }

        return image;
    }

    private static Image? MutateImage(Image? image, string text, Font font, Color colour, PointF position)
    {
        image.Mutate(op => op.DrawText(text, font, colour, position));
        return image;
    }
}