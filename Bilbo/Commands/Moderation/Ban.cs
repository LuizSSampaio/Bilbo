using Bilbo.Models;
using Bilbo.Services;
using Discord;
using Discord.WebSocket;

namespace Bilbo.Commands.Moderation;

public class Ban : CommandFramework
{
    private readonly DiscordSocketClient _client;
    
    private new static readonly List<Option> Options = new()
    {
        new Option("user", ApplicationCommandOptionType.User, "The user to ban", true),
        new Option("reason", ApplicationCommandOptionType.String, "The reason for the ban", false),
        new Option("days", ApplicationCommandOptionType.Integer, "The number of days of messages to delete", false)
    };
    
    public Ban(DiscordSocketClient client) : base("ban", "Ban a user from the server", Options)
    {
        _client = client;
    }

    // TODO: Role based permissions(Ex: mods can ban only with the bot) 
    // TODO: Command response on a specific channel(Configurable at the website)
    public override void CommandAction(SocketSlashCommand command)
    {
        if (command.User is not SocketGuildUser commandUser || !commandUser.GuildPermissions.BanMembers)
        {
            command.RespondAsync("You do not have permission to use this command.");
            return;
        }
        
        var user = command.Data.Options.First().Value as SocketGuildUser;
        var reason = command.Data.Options.ElementAt(1).Value as string;
        var days = command.Data.Options.Last().Value as int? ?? 0;
        var guild = _client.GetGuild(command.GuildId!.Value);

        var embedBuilder = new CustomEmbedBuilder();
        
        embedBuilder.WithTitle("🔨 Ban");
        embedBuilder.WithDescription($"You have been banned from {guild}.");
        embedBuilder.AddField("Reason: ", reason ?? "No reason provided.");
        embedBuilder.WithColor(Color.Red);

        embedBuilder.WithFooter(command.User.Discriminator != "0000"
            ? $"Banned by {command.User.Username}#{command.User.Discriminator}"
            : $"Banned by {command.User.Username}");

        user.SendMessageAsync("", false, embedBuilder.Build());
        user.BanAsync(days, reason);
        
        embedBuilder.WithDescription($"{user?.Mention} has been banned.");
        command.RespondAsync("", new[] { embedBuilder.Build() });
    }
}