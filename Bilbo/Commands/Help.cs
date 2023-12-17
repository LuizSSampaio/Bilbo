using Bilbo.Services;
using Discord.WebSocket;

namespace Bilbo.Commands;

public class Help : CommandFramework
{
    public Help() : base("help", "The place to get help with your questions", null)
    {
    }

    public override async void CommandAction(SocketSlashCommand command)
    {
        if (command.Channel is SocketGuildChannel guildChannel &&
            !BotPermissionChecker.CanSendMessageToChannel(command, guildChannel))
        {
            return;
        }
        
        var embedBuilder = new CustomEmbedBuilder();

        embedBuilder.WithTitle("🔮 Help");
        embedBuilder.WithDescription("Bilbo is a web centred bot, so you can find all the commands on my website.");
        embedBuilder.AddField("🖥️ Website: ", "[Click Here](https://bilbo.lsamp.net)", true);
        embedBuilder.AddField("📢 Support Server: ", "[Click Here](https://http.cat/404)", true);
        embedBuilder.AddField("📚 Documentation: ", "[Click Here](https://bilbo.lsamp.net/docs)", true);
        embedBuilder.AddField("📊 Status: ", "[Click Here](https://uptime.lsamp.net/status/bilbo)", true);
        embedBuilder.AddField("📝 Source Code: ", "[Click Here](https://github.com/LuizSSampaio/Bilbo)", true);
        embedBuilder.AddField("📜 License: ",
            "[Click Here](https://github.com/LuizSSampaio/Bilbo/blob/main/LICENSE.md)", true);
        embedBuilder.WithFooter(
            "Warning: This bot is still in development, so some commands may not work as expected.");

        try
        {
            await command.RespondAsync("", new[] { embedBuilder.Build() }, ephemeral: true);
        }
        catch
        {
            var genericErrorMessage = new GenericErrorMessage();
            try
            {
                await command.RespondAsync("", new[] { genericErrorMessage.Build() }, ephemeral: true);
            }
            catch
            {
                // ignored
            }
        }
    }
}