using Discord;

namespace Bilbo.Models;

public struct Option
{
    internal string Name { get; }
    internal ApplicationCommandOptionType Type { get; }
    internal string Description { get; }
    internal bool Required { get; }

    public Option(string name, ApplicationCommandOptionType type, string description, bool required)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Required = required;
    }
}