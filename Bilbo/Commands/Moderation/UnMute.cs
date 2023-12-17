using Bilbo.Models;
using Bilbo.Services;
using Discord;
using Discord.WebSocket;

namespace Bilbo.Commands.Moderation;

public class UnMute : CommandFramework
{
    private readonly DiscordSocketClient _client;

    private new static readonly List<Option> Options = new()
    {
        new Option("user", ApplicationCommandOptionType.User, "The user to unmute", true),
        new Option("reason", ApplicationCommandOptionType.String, "The reason for the unmute", false),
    };
    
    public UnMute(DiscordSocketClient client) : base("unmute", "Unmute a user from the server", Options)
    {
        _client = client;
    }

    public override async void CommandAction(SocketSlashCommand command)
    {
        var authorPermissionChecker =
            new AuthorPermissionChecker(command, new[] { GuildPermission.MuteMembers }, null, null);

        if (!authorPermissionChecker.HasPermissions)
        {
            return;
        }
        
        var user = command.Data.Options.First().Value as SocketGuildUser;
        var reason = command.Data.Options.Last().Value as string;
        var guild = _client.GetGuild(command.GuildId!.Value);
        
        var embedBuilder = new CustomEmbedBuilder();
        
        embedBuilder.WithTitle("🔊 Unmute");
        embedBuilder.WithDescription($"You have been unmuted from {guild}.");
        embedBuilder.AddField("Reason: ", reason ?? "No reason provided.");
        
        embedBuilder.CustomIdentifiedFooter("Unmuted", command.User);
        
        await user.SendMessageAsync("", false, embedBuilder.Build());
        await user!.RemoveTimeOutAsync(new RequestOptions { AuditLogReason = reason });
        
        embedBuilder.WithDescription($"{user?.Mention} has been unmuted.");
        await command.RespondAsync("", new[] { embedBuilder.Build() });
    }
}