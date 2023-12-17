using Bilbo.Models;
using Bilbo.Services;
using Discord;
using Discord.WebSocket;
using Exception = System.Exception;

namespace Bilbo.Commands.Moderation;

public class Kick : CommandFramework
{
    private readonly DiscordSocketClient _client;
    
    private new static readonly List<Option> Options = new()
    {
        new Option("user", ApplicationCommandOptionType.User, "The user to kick", true),
        new Option("reason", ApplicationCommandOptionType.String, "The reason for the kick", false),
    };
    
    public Kick(DiscordSocketClient client) : base("kick", "Kick a user from the server", Options)
    {
        _client = client;
    }

    // TODO: Role based permissions(Ex: mods can kick only with the bot) 
    // TODO: Command response on a specific channel(Configurable at the website)
    public override async void CommandAction(SocketSlashCommand command)
    {
        var authorPermissionChecker =
            new AuthorPermissionChecker(command, new[] { GuildPermission.KickMembers }, null, null);

        if (!authorPermissionChecker.HasPermissions)
        {
            return;
        }
        
        if (command.Channel is SocketGuildChannel guildChannel &&
            !BotPermissionChecker.CanSendMessageToChannel(command, guildChannel))
        {
            return;
        }
        
        if (!BotPermissionChecker.BotCheckDiscord(command, _client, new[] { GuildPermission.KickMembers }))
        {
            return;
        }
        
        var user = command.Data.Options.First().Value as SocketGuildUser;
        var reason = command.Data.Options.Last().Value as string;
        var guild = _client.GetGuild(command.GuildId!.Value);
        
        var embedBuilder = new CustomEmbedBuilder();
        
        embedBuilder.WithTitle("👢 Kick");
        embedBuilder.WithDescription($"You have been kicked from {guild}.");
        embedBuilder.AddField("Reason: ", reason ?? "No reason provided.");
        embedBuilder.WithColor(Color.Red);
        
        embedBuilder.CustomIdentifiedFooter("Kicked", command.User);

        try
        {
            await user.SendMessageAsync("", false, embedBuilder.Build());
        }
        catch
        {
            // ignored
        }

        try
        {
            user?.KickAsync(reason);
        }
        catch
        {
            var genericErrorMessage = new GenericErrorMessage("The user has a higher role than the bot.");
            await command.RespondAsync("", new[] { genericErrorMessage.Build() }, ephemeral: true);
            return;
        }
        
        embedBuilder.WithDescription($"{user?.Mention} has been kicked.");
        await command.RespondAsync("", new[] { embedBuilder.Build() });
    }
}