using Discord;

namespace Bilbo.Services;

public class GenericErrorMessage : CustomEmbedBuilder
{
    public GenericErrorMessage() : base()
    {
        WithTitle("❌ Error");
        WithDescription("An unexpected error has occurred.");
        AddField("Report on GitHub: ", "[Click Here](https://github.com/LuizSSampaio/Bilbo/issues)");
        WithColor(new Color(255, 0, 0));
    }
    
    public GenericErrorMessage(string errorMessage) : base()
    {
        WithTitle("❌ Error");
        WithDescription(errorMessage);
        WithColor(new Color(255, 0, 0));
    }
}