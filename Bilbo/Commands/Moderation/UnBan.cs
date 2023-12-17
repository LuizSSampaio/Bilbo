using Bilbo.Models;
using Bilbo.Services;
using Discord;
using Discord.WebSocket;

namespace Bilbo.Commands.Moderation;

public class UnBan : CommandFramework
{
    private readonly DiscordSocketClient _client;
    
    private new static readonly List<Option> Options = new()
    {
        new Option("user-id", ApplicationCommandOptionType.String, "The user id to unban", true),
        new Option("reason", ApplicationCommandOptionType.String, "The reason for the unban", false),
    };
    
    public UnBan(DiscordSocketClient client) : base("unban", "Unban a user from the server", Options)
    {
        _client = client;
    }

    private static async void ErrorMessage(SocketInteraction command, string error)
    {
        var embedBuilder = new CustomEmbedBuilder();
        
        embedBuilder.WithTitle("🔨 Unban");
        embedBuilder.WithDescription(error);
        embedBuilder.WithColor(Color.Red);
        
        await command.RespondAsync("", new[] { embedBuilder.Build() }, ephemeral: true);
    }
    
    public override async void CommandAction(SocketSlashCommand command)
    {
        var authorPermissionChecker =
            new AuthorPermissionChecker(command, new[] { GuildPermission.BanMembers }, null, null);

        if (!authorPermissionChecker.HasPermissions)
        {
            return;
        }
        
        var userId = command.Data.Options.First().Value as string;
        var reason = command.Data.Options.Last().Value as string;
        var guild = _client.GetGuild(command.GuildId!.Value);
        
        var embedBuilder = new CustomEmbedBuilder();
        
        embedBuilder.WithTitle("🔨 Unban");
        embedBuilder.WithDescription($"You have been unbanned from {guild}.");
        embedBuilder.AddField("Reason: ", reason ?? "No reason provided.");
        
        embedBuilder.CustomIdentifiedFooter("Unbanned", command.User);

        if (userId != null)
        {
            try
            {
                await guild.RemoveBanAsync(ulong.Parse(userId), new RequestOptions { AuditLogReason = reason });
            }
            catch
            {
                ErrorMessage(command, "The user id is not valid for unban command.");
                return;
            }
            var user = _client.GetUser(userId);
            if (user != null)
            {
                await user.SendMessageAsync("", false, embedBuilder.Build());

            }

            embedBuilder.WithDescription($"{userId} has been unbanned.");
        }
        else
        {
            ErrorMessage(command, "The user id is not valid.");
            return;
        }

        await command.RespondAsync("", new[] { embedBuilder.Build() });
    }
}