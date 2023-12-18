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
    
    // TODO: Role based permissions(Ex: mods can ban only with the bot)
    // TODO: Command response on a specific channel(Configurable at the website)
    public override async void CommandAction(SocketSlashCommand command)
    {
        var authorPermissionChecker =
            new AuthorPermissionChecker(command, new[] { GuildPermission.BanMembers }, null, null);

        if (!authorPermissionChecker.HasPermissions)
        {
            return;
        }
        
        if (command.Channel is SocketGuildChannel guildChannel &&
            !BotPermissionChecker.CanSendMessageToChannel(command, guildChannel))
        {
            return;
        }
        
        if (!BotPermissionChecker.BotCheckDiscord(command, _client, new[] { GuildPermission.BanMembers }))
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
                var genericErrorMessage = new GenericErrorMessage("The user id is not valid for unban command.");
                await command.RespondAsync("", new[] { genericErrorMessage.Build() });
                return;
            }
            var user = _client.GetUser(userId);
            if (user != null)
            {
                try
                {
                    await user.SendMessageAsync("", false, embedBuilder.Build());
                }
                catch
                {
                    // ignored
                }
            }

            embedBuilder.WithDescription($"{userId} has been unbanned.");
        }
        else
        {
            var genericErrorMessage = new GenericErrorMessage("The user id is not valid.");
            await command.RespondAsync("", new[] { genericErrorMessage.Build() });
            return;
        }

        await command.RespondAsync("", new[] { embedBuilder.Build() });
    }
}