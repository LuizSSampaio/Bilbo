using Bilbo.Models;
using Discord.WebSocket;

namespace Bilbo.Services;

public abstract class CommandFramework
{
    internal string Name { get; }
    internal string Description { get; }
    internal List<Option>? Options { get; }

    protected CommandFramework(string command, string description, List<Option>? options)
    {
        Name = command ?? throw new ArgumentNullException(nameof(command));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Options = options;
        
        CommandLocation.GlobalCommands.Add(this);
    }

    public abstract void CommandAction(SocketSlashCommand command);
}