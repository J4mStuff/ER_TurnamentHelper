using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Tournaments;

public class ImageDrawer
{
    private const string TemplateFileName = "template.png";
    private const string OutputDirectory = "out";
    private readonly Font _font = SystemFonts.CreateFont("Arial", 35);

    private const float StartingX = 430;
    private const float StartingY = 200;

    public void PopulateTemplate(List<GameStats> statsList)
    {
        Initialize();
        const string outFile = "1";
        DrawStats(statsList, outFile);
    }

    private static void Initialize()
    {
        Directory.CreateDirectory(OutputDirectory);
    }

    private void DrawStats(List<GameStats> statsList, string outFileName)
    {
        var image = Image.Load(TemplateFileName);

        var positionMultiplier = 1;
        foreach (var stats in statsList)
        {
            var (x, y) = (StartingX, StartingY + positionMultiplier*50);
            
            image.Mutate(op => op.DrawText(stats.Name, _font, Color.White, new PointF(x, y)));
            image.Mutate(op => op.DrawText(stats.Placements.ToString(), _font, Color.White, new PointF(x+825, y)));
            image.Mutate(op => op.DrawText(stats.Kills.ToString(), _font, Color.White, new PointF(x+925, y)));
            image.Mutate(op => op.DrawText(stats.Score.ToString(), _font, Color.White, new PointF(x+1075, y)));
            
            positionMultiplier++;
        }
        image.Save($"{OutputDirectory}/{outFileName}.png");
    }
}