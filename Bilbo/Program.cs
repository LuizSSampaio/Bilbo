using System.Collections;
using Discord;
using Discord.WebSocket;

public class Program
{
    private readonly DiscordSocketClient _client;

    public static Task Main(string[] args) => new Program().MainAsync();

    public Program()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient();
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;
        _client.InteractionCreated += InteractionCreatedAsync;
    }

    public async Task MainAsync()
    {
        var token = Environment.GetEnvironmentVariable("BILBO_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine(token);
            throw new ArgumentNullException("BILBO_TOKEN",
                "The TOKEN environment variable is not set or is set to an invalid value.");
        }

        await _client.LoginAsync(TokenType.Bot, token);

        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} is connected!");
        return Task.CompletedTask;
    }

    private Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.Id == _client.CurrentUser.Id)
        {
            return Task.CompletedTask;
        }

        if (message.Content == "!ping")
        {
            var componentBuilder = new ComponentBuilder()
                .WithButton("Click me!", "click_me", ButtonStyle.Primary);
            message.Channel.SendMessageAsync("Pong!", components: componentBuilder.Build());
        }

        return Task.CompletedTask;
    }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        if (interaction is not SocketMessageComponent messageComponent)
        {
            return;
        }

        if (messageComponent.Data.CustomId == "click_me")
        {
            await messageComponent.UpdateAsync(x => x.Content = "Clicked!");
        }
        else
        {
            await messageComponent.UpdateAsync(x => x.Content = "Something else was clicked!");
        }
    }
}