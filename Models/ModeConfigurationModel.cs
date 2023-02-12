

// ReSharper disable ClassNeverInstantiated.Global

using Enums;

namespace Models;

public class ModeConfigurationModel
{
    public ModeConfigurationModel()
    {
        Name = string.Empty;
        FontName = string.Empty;
        FontColour = new Dictionary<ColourCodes, byte>();
        TemplateConfiguration = new TemplateConfigurationModel();
        KillMultiplier = new KillMultiplierModel();
        PlacementScoring = new Dictionary<string, int>();
    }

    public string Name { get; set; }
    public string FontName { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public Dictionary<ColourCodes, byte> FontColour { get; set; }
    public TemplateConfigurationModel TemplateConfiguration { get; set; }
    public KillMultiplierModel KillMultiplier { get; set; }
    public Dictionary<string, int> PlacementScoring { get; set; }
}