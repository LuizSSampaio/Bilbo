using Bilbo.Services;
using Discord.WebSocket;

namespace Bilbo.Commands;

public class Status : CommandFramework
{
    public Status() : base("status", "Test", null)
    {
    }

    public override async void CommandAction(SocketSlashCommand command)
    {
        await command.RespondAsync("Test");
    }
}