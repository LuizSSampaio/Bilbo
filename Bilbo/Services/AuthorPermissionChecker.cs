using Discord;
using Discord.WebSocket;

namespace Bilbo.Services;

public class AuthorPermissionChecker
{
    internal bool HasPermissions { get; }

    public AuthorPermissionChecker(SocketInteraction command, IEnumerable<GuildPermission>? permissions,
        IEnumerable<IUser>? users, IEnumerable<IRole>? roles)
    {
        HasPermissions = CheckDiscord(permissions, command) || CheckUser(users, command) || CheckRole(roles, command);
    }

    private static void MissingPermissionsResponse(SocketInteraction command, List<GuildPermission> permissions)
    {
        var embedBuilder = new CustomEmbedBuilder();

        embedBuilder.WithTitle("🔒 Missing Permissions");
        embedBuilder.WithDescription("You are missing the following permissions to use this command:");

        foreach (var permission in permissions)
        {
            embedBuilder.AddField(permission.ToString(), "You do not have this permission.");
        }

        command.RespondAsync("", new[] { embedBuilder.Build() }, ephemeral: true);
    }

    private static void NotInGuildResponse(SocketInteraction command)
    {
        var embedBuilder = new CustomEmbedBuilder();

        embedBuilder.WithTitle("🔒 Missing Permissions");
        embedBuilder.WithDescription("You are missing the following permissions to use this command:");
        embedBuilder.AddField("Guild", "You are not in a guild.");

        command.RespondAsync("", new[] { embedBuilder.Build() }, ephemeral: true);
    }

    private static bool CheckDiscord(IEnumerable<GuildPermission>? permissions, SocketInteraction command)
    {
        // True because if the permissions are null, it means that the command does not require any permissions.
        if (permissions is null) return true;

        if (command.User is not SocketGuildUser commandUser)
        {
            NotInGuildResponse(command);
            return false;
        }

        var missingPermissions =
            permissions.Where(permission => !commandUser.GuildPermissions.Has(permission)).ToList();

        if (missingPermissions.Count <= 0) return true;

        MissingPermissionsResponse(command, missingPermissions);
        return false;
    }

    private static bool CheckUser(IEnumerable<IUser>? users, SocketInteraction command)
    {
        // False because if the users are null, it means that the command doesn't have any user whitelisted.
        if (users is null) return false;

        if (command.User is SocketGuildUser commandUser) return users.Any(user => commandUser.Id == user.Id);
        NotInGuildResponse(command);
        return false;
    }

    private static bool CheckRole(IEnumerable<IRole>? roles, SocketInteraction command)
    {
        // False because if the roles are null, it means that the command doesn't have any role whitelisted.
        if (roles is null) return false;

        if (command.User is SocketGuildUser commandUser)
        {
            var userRoles = new HashSet<ulong>(commandUser.Roles.Select(role => role.Id));
            return roles.Any(role => userRoles.Contains(role.Id));
        }

        NotInGuildResponse(command);
        return false;
    }
}