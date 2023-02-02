using Tournaments.Enums;

namespace Tournaments.Models;

public class ModeConfiguration
{
    public string Name { get; set; }
    public string FontName { get; set; }
    public Dictionary<ColourCodes, byte> FontColour { get; set; }
    public TemplateModeSettings TemplateConfiguration { get; set; }
    public int killsMultiplier { get; set; }
    public Dictionary<string,int> PlacementScoring { get; set; }
}