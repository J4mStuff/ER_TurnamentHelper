

// ReSharper disable ClassNeverInstantiated.Global

using Enums;

namespace Models;

public class ModeConfiguration
{
    public ModeConfiguration()
    {
        Name = string.Empty;
        FontName = string.Empty;
        FontColour = new Dictionary<ColourCodes, byte>();
        TemplateConfiguration = new TemplateModeSettings();
        KillsMultiplier = 1;
        PlacementScoring = new Dictionary<string, int>();
    }

    public string Name { get; set; }
    public string FontName { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public Dictionary<ColourCodes, byte> FontColour { get; set; }
    public TemplateModeSettings TemplateConfiguration { get; set; }
    public int KillsMultiplier { get; set; }
    public Dictionary<string, int> PlacementScoring { get; set; }
}