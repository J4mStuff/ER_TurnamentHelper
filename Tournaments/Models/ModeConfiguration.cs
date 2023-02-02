using Tournaments.Workflow;

namespace Tournaments.Models;

public class ModeConfiguration
{
    public string Name { get; set; }
    public int KillsMultiplier { get; set; }
    public int FontSize { get; set; }
    public string FontName { get; set; }
    public TemplateModeSettings TemplateConfiguration { get; set; }
    public Dictionary<string,int> PlacementScoring { get; set; }
}