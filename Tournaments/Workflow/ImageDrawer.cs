using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using Tournaments.Enums;
using Tournaments.Models;

namespace Tournaments.Workflow;

public class ImageDrawer
{
    private ModeConfiguration _modeConfiguration = new();
    private const string OutputDirectory = "leaderboards";

    public void PopulateTemplate(List<GameStats> statsList, ModeConfiguration config)
    {
        _modeConfiguration = config;
        Directory.CreateDirectory(OutputDirectory);
        DrawStats(statsList, $"{_modeConfiguration.Name}{_modeConfiguration.TemplateConfiguration.SummarySuffix}");
        DrawStats(statsList, $"{_modeConfiguration.Name}{_modeConfiguration.TemplateConfiguration.LastGameSuffix}");
    }

    private void DrawStats(IReadOnlyCollection<GameStats> statsList, string outFileName)
    {
        var image = Image.Load(Path.Combine("Assets", _modeConfiguration.TemplateConfiguration.TemplateFileName));

        var columns = _modeConfiguration.TemplateConfiguration.Columns.Count;
        var entriesPerColum = statsList.Count / columns;

        var startingY = _modeConfiguration.TemplateConfiguration.YStartingPosition;

        var processed = 0;
        var chunk = statsList.Skip(processed).Take(entriesPerColum).ToList();
        foreach (var column in _modeConfiguration.TemplateConfiguration.Columns)
        {
            image = DrawStats2(image, column, chunk, startingY);
            processed += chunk.Count;
        }
        
        image.Save($"{OutputDirectory}/{outFileName}.png");
    }

    private Image? DrawStats2(Image? image, ColumnData columnData, List<GameStats> statsList, int startingY)
    {
        var colour = Color.FromRgb(_modeConfiguration.FontColour[ColourCodes.R], _modeConfiguration.FontColour[ColourCodes.G], _modeConfiguration.FontColour[ColourCodes.B]);
        var multiplier = 1;
        foreach (var stats in statsList)
        {
            var y = startingY + multiplier * _modeConfiguration.TemplateConfiguration.NewLineDistance;

            image = MutateImage(image, stats.PlayerName,
                SystemFonts.CreateFont(_modeConfiguration.FontName, columnData.NameField.FontSize), colour,
                new PointF(columnData.NameField.XPosition, y));
            image = MutateImage(image, stats.Placements.ToString(),
                SystemFonts.CreateFont(_modeConfiguration.FontName, columnData.PlacementField.FontSize), colour,
                new PointF(columnData.PlacementField.XPosition, y));
            image = MutateImage(image, stats.Kills.ToString(),
                SystemFonts.CreateFont(_modeConfiguration.FontName, columnData.KillsField.FontSize), colour,
                new PointF(columnData.KillsField.XPosition, y));
            image = MutateImage(image, stats.Score.ToString(),
                SystemFonts.CreateFont(_modeConfiguration.FontName, columnData.ScoreField.FontSize), colour,
                new PointF(columnData.ScoreField.XPosition, y));
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