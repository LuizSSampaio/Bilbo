﻿using Bilbo.Commands;
using Bilbo.Commands.Moderation;
using Bilbo.Models;
using Discord;
using Discord.WebSocket;

namespace Bilbo;

public class Program
{
    private readonly DiscordSocketClient _client;

    // Global Commands
    private Status _statusCommand;
    private Help _helpCommand = new();

    // Moderation Commands
    private Ban _banCommand;
    private UnBan _unBanCommand;
    private Kick _kickCommand;
    private Mute _muteCommand;
    private UnMute _unMuteCommand;

    public static Task Main(string[] args) => new Program().MainAsync();

    private Program()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);

        _statusCommand = new Status(_client);
        _banCommand = new Ban(_client);
        _unBanCommand = new UnBan(_client);
        _kickCommand = new Kick(_client);
        _muteCommand = new Mute(_client);
        _unMuteCommand = new UnMute(_client);

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.SlashCommandExecuted += SlashCommandHandler;
        _client.InteractionCreated += InteractionCreatedAsync;
    }

    private async Task MainAsync()
    {
        var token = Environment.GetEnvironmentVariable("BILBO_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
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

    private async Task<Task> ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} is connected!");

        foreach (var command in CommandLocation.GlobalCommands)
        {
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName(command.Name);
            globalCommand.WithDescription(command.Description);
            if (command.Options != null)
            {
                foreach (var option in command.Options)
                {
                    globalCommand.AddOption(option.Name, option.Type, option.Description, option.Required,
                        choices: option.Choices);
                }
            }

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                throw;
            }
        }

        return Task.CompletedTask;
    }

    private Task SlashCommandHandler(SocketSlashCommand command)
    {
        var usedCommand = CommandLocation.GlobalCommands
            .FirstOrDefault(globalCommand => globalCommand.Name == command.Data.Name);

        usedCommand?.CommandAction(command);

        return Task.CompletedTask;
    }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
    }
}