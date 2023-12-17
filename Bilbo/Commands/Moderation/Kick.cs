using Bilbo.Models;
using Bilbo.Services;
using Discord;
using Discord.WebSocket;

namespace Bilbo.Commands.Moderation;

public class Kick : CommandFramework
{
    private readonly DiscordSocketClient _client;
    
    private new static readonly List<Option> Options = new()
    {
        new Option("user", ApplicationCommandOptionType.User, "The user to ban", true),
        new Option("reason", ApplicationCommandOptionType.String, "The reason for the kick", false),
    };
    
    public Kick(DiscordSocketClient client) : base("kick", "Kick a user from the server", Options)
    {
        _client = client;
    }

    // TODO: Role based permissions(Ex: mods can kick only with the bot) 
    // TODO: Command response on a specific channel(Configurable at the website)
    public override void CommandAction(SocketSlashCommand command)
    {
        if (command.User is not SocketGuildUser commandUser || !commandUser.GuildPermissions.KickMembers)
        {
            command.RespondAsync("You do not have permission to use this command.");
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
        
        embedBuilder.WithFooter(command.User.Discriminator != "0000"
            ? $"Kicked by {command.User.Username}#{command.User.Discriminator}"
            : $"Kicked by {command.User.Username}");
        
        user.SendMessageAsync("", false, embedBuilder.Build());
        user?.KickAsync(reason);
        
        embedBuilder.WithDescription($"{user?.Mention} has been kicked.");
        command.RespondAsync("", new[] { embedBuilder.Build() });
    }
}