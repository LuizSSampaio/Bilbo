using System.Diagnostics;
using Bilbo.Services;
using Discord;
using Discord.WebSocket;

namespace Bilbo.Commands;

public class Status : CommandFramework
{
    private readonly DiscordSocketClient _client;

    public Status(DiscordSocketClient client) : base("status", "A simple command to see the bot status", null)
    {
        _client = client;
    }

    private static string GetUptime()
    {
        var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
        var uptimeInDays = uptime.Days;
        var uptimeHoursMinutesAndSeconds = uptime.ToString(@"hh\:mm\:ss");
        return $"{uptimeInDays} days & {uptimeHoursMinutesAndSeconds}";
    }

    private int GetGuildCount()
    {
        return _client.Guilds.Count;
    }

    private int GetUsersCount()
    {
        return _client.Guilds.Sum(guild => guild.MemberCount);
    }

    private int GetLatency()
    {
        return _client.Latency;
    }

    private int GetChannelCount()
    {
        return _client.Guilds.Sum(guild => guild.Channels.Count(c => c is not SocketCategoryChannel));
    }

    public override async void CommandAction(SocketSlashCommand command)
    {
        var builder = new EmbedBuilder();

        builder.WithAuthor("❤️ With love from Bilbo", null, "https://github.com/LuizSSampaio/Bilbo");
        builder.WithTitle("📊 Status");
        builder.WithDescription("[Click here](https://uptime.lsamp.net/status/bilbo) to check the status website.");

        builder.AddField("⏳ Uptime: ", GetUptime(), true);
        builder.AddField("📡 Latency: ", $"{GetLatency()}ms", true);
        builder.AddField("💻 Number of Servers: ", GetGuildCount(), true);
        builder.AddField("🙍 Number of Users: ", GetUsersCount(), true);
        builder.AddField("📢 Number of Channels: ", GetChannelCount(), true);
        builder.AddField("🖥️ Check my website: ", "[Click Here](https://bilbo.lsamp.net)", true);

        builder.WithTimestamp(DateTimeOffset.Now);
        builder.WithFooter("Need help? Check out the /help command.");

        builder.WithColor(Color.Blue);

        await command.RespondAsync("", new[] { builder.Build() });
    }
}