using Discord;

namespace Bilbo.Models;

public struct Option
{
    private string Name { get; set; }
    private ApplicationCommandOptionType Type { get; set; }
    private string Description { get; set; }
    private bool Required { get; set; }

    public Option(string name, ApplicationCommandOptionType type, string description, bool required)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Required = required;
    }
}