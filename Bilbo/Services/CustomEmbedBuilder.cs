using Discord;

namespace Bilbo.Services;

public class CustomEmbedBuilder : EmbedBuilder
{
    public CustomEmbedBuilder()
    {
        WithAuthor("❤️ With love from Bilbo", null, "https://github.com/LuizSSampaio/Bilbo");
        WithColor(new Color(119, 221, 119));
        WithTimestamp(DateTimeOffset.Now);
        WithFooter("Need help? Check out the /help command.");
    }
}