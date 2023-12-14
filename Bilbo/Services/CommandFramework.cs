using Bilbo.Models;

namespace Bilbo.Services;

public abstract class CommandFramework
{
    private string _command;
    private string _description;
    private Option _option;
    private bool _isGlobal;

    protected CommandFramework(string command, string description, Option option, bool isGlobal)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _description = description ?? throw new ArgumentNullException(nameof(description));
        _option = option;
        _isGlobal = isGlobal;
        
        if (isGlobal)
        {
            CommandLocation.GlobalCommands.Add(this);
        }
        else
        {
            CommandLocation.GuildCommands.Add(this);
        }
    }

    public abstract void CommandAction();
}