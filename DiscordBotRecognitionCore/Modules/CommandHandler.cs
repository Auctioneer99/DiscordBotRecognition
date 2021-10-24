using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Modules
{
    public class CommandHandler
    {
        public const char PREFIX = '#';

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _commands = commands;
            _client = client;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
            _commands.CommandExecuted += OnCommandExecutedAsync;
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                await context.Channel.SendMessageAsync($"```{result.ErrorReason}```");
                Console.WriteLine(result.ErrorReason);
            }
            var commandName = command.IsSpecified ? command.Value.Name : "A command";
            Console.WriteLine(new LogMessage(LogSeverity.Info,
                "CommandExecution",
                $"{commandName} was executed at {DateTime.UtcNow}."));
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            int argPos = 0;

            if (!(message.HasCharPrefix(PREFIX, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }
}
