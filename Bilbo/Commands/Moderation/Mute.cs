using Bilbo.Models;
using Bilbo.Services;
using Discord;
using Discord.WebSocket;

namespace Bilbo.Commands.Moderation;

public class Mute : CommandFramework
{
    private readonly DiscordSocketClient _client;

    private static readonly ApplicationCommandOptionChoiceProperties[] Choices =
    {
        new() // seconds
        {
            Name = "seconds",
            Value = 0,
            NameLocalizations = null
        },
        new() // minutes
        {
            Name = "minutes",
            Value = 1,
            NameLocalizations = null
        },
        new() // hours
        {
            Name = "hours",
            Value = 2,
            NameLocalizations = null
        },
        new() // days
        {
            Name = "days",
            Value = 3,
            NameLocalizations = null
        },
        new() // weeks
        {
            Name = "weeks",
            Value = 4,
            NameLocalizations = null
        },
    };

    private new static readonly List<Option> Options = new()
    {
        new Option("user", ApplicationCommandOptionType.User, "The user to ban", true),
        new Option("time-type", ApplicationCommandOptionType.Integer, "The time type to mute the user", true, Choices),
        new Option("value", ApplicationCommandOptionType.Integer, "The time value to mute the user", true),
        new Option("reason", ApplicationCommandOptionType.String, "The reason for the kick", false),
    };

    public Mute(DiscordSocketClient client) : base("mute", "Mute a user from the server", Options)
    {
        _client = client;
    }

    private static async Task<bool> MaxValueCheck(SocketInteraction command, long value, long maxValue)
    {
        if (value <= maxValue) return true;
        var embedBuilder = new CustomEmbedBuilder();
            
        embedBuilder.WithTitle("🔇 Mute");
        embedBuilder.WithDescription($"The value must be less or equal than {maxValue}.");
        embedBuilder.WithColor(Color.Red);
            
        await command.RespondAsync("", new[] { embedBuilder.Build() }, ephemeral: true);
        return false;
    }
    
    // TODO: Role based permissions(Ex: mods can mute only with the bot) 
    // TODO: Command response on a specific channel(Configurable at the website)
    // TODO: Add a button to contest the mute
    public override async void CommandAction(SocketSlashCommand command)
    {
        var authorPermissionChecker =
            new AuthorPermissionChecker(command, new[] { GuildPermission.ModerateMembers }, null, null);

        if (!authorPermissionChecker.HasPermissions)
        {
            return;
        }
        
        if (command.Channel is SocketGuildChannel guildChannel &&
            !BotPermissionChecker.CanSendMessageToChannel(command, guildChannel))
        {
            return;
        }
        
        if (!BotPermissionChecker.BotCheckDiscord(command, _client, new[] { GuildPermission.ModerateMembers }))
        {
            return;
        }

        var user = command.Data.Options.First().Value as SocketGuildUser;
        var timeType = (long)command.Data.Options.ElementAt(1).Value;
        var value = (long)command.Data.Options.ElementAt(2).Value;
        var reason = command.Data.Options.Last().Value as string;
        var guild = _client.GetGuild(command.GuildId!.Value);
        
        if (value <= 0)
        {
            await command.RespondAsync("The value must be greater than 0.");
            return;
        }

        switch (timeType)
        {
            case 0:
                if (!MaxValueCheck(command, value, 60).Result)
                {
                    return;
                }
                break;
            case 1:
                if (!MaxValueCheck(command, value, 60).Result)
                {
                    return;
                }
                break;
            case 2:
                if (!MaxValueCheck(command, value, 24).Result)
                {
                    return;
                }
                break;
            case 3:
                if (!MaxValueCheck(command, value, 7).Result)
                {
                    return;
                }
                break;
            case 4:
                if (!MaxValueCheck(command, value, 4).Result)
                {
                    return;
                }
                break;
        }

        var embedBuilder = new CustomEmbedBuilder();

        embedBuilder.WithTitle("🔇 Mute");
        embedBuilder.WithDescription($"You have been muted from {guild}.");
        embedBuilder.AddField("Duration: ", $"{value} {Choices[timeType].Name}");
        embedBuilder.AddField("Reason: ", reason ?? "No reason provided.");
        embedBuilder.WithColor(Color.Red);
        
        embedBuilder.CustomIdentifiedFooter("Muted", command.User);

        try
        {
            await user.SendMessageAsync("", false, embedBuilder.Build());
        }
        catch
        {
            // ignored
        }

        var timeSpan = timeType switch
        {
            0 => TimeSpan.FromSeconds(value),
            1 => TimeSpan.FromMinutes(value),
            2 => TimeSpan.FromHours(value),
            3 => TimeSpan.FromDays(value),
            4 => TimeSpan.FromDays(value * 7),
            _ => TimeSpan.Zero
        };
        try
        {
            await user!.SetTimeOutAsync(timeSpan, new RequestOptions { AuditLogReason = reason });
        }
        catch
        {
            var genericErrorMessage = new GenericErrorMessage("The user has a higher role than the bot.");
            await command.RespondAsync("", new[] { genericErrorMessage.Build() }, ephemeral: true); 
            return;
        }

        embedBuilder.WithDescription($"{user.Mention} has been muted.");
        await command.RespondAsync("", new[] { embedBuilder.Build() });
    }
}