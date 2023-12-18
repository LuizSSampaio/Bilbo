using Discord;
using Discord.WebSocket;

namespace Bilbo.Services;

public class BotPermissionChecker
{
    private static void BotMissingPermissionsResponse(SocketInteraction command, List<GuildPermission> permissions)
    {
        var embedBuilder = new CustomEmbedBuilder();

        embedBuilder.WithTitle("🔒 Missing Permissions");
        embedBuilder.WithDescription("I am missing the following permissions to use this command:");

        foreach (var permission in permissions)
        {
            embedBuilder.AddField(permission.ToString(), "I do not have this permission.");
        }

        command.RespondAsync("", new[] { embedBuilder.Build() }, ephemeral: true);
    }

    public static bool CanSendMessageToChannel(SocketInteraction command, SocketChannel channel)
    {
        if (channel is not SocketGuildChannel guildChannel)
        {
            return false;
        }

        if (guildChannel.Guild.CurrentUser.GetPermissions(guildChannel).SendMessages) return true;
        var commandUser = command.User as SocketGuildUser;
        var embedBuilder = new CustomEmbedBuilder();

        embedBuilder.WithTitle("🔒 Missing Permissions");
        embedBuilder.WithDescription("I am missing the following permissions to use this command:");
        embedBuilder.AddField("Send Messages", "I do not have this permission.");

        try
        {
            commandUser.SendMessageAsync("", false, embedBuilder.Build());
        }
        catch
        {
            // ignored
        }
        return false;
    }

    public static bool BotCheckDiscord(SocketInteraction command, DiscordSocketClient client,
        IEnumerable<GuildPermission>? permissions)
    {
        if (permissions is null) return true;
        
        var missingPermissions =
            permissions.Where(permission => !client.Guilds.First().CurrentUser.GuildPermissions.Has(permission)).ToList();
        
        if (missingPermissions.Count <= 0) return true;
        
        BotMissingPermissionsResponse(command, missingPermissions);
        return false;
    }
}