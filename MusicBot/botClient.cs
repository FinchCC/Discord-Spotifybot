using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using MusicBot.Dependencys;

namespace MusicBot
{
    public class botClient
    {
        private DiscordSocketClient _client;
        private CommandService _commandsService;
        private ServiceProvider _serviceProvider; //costum support to custom objects??
        private LavaNode _node;
        private MusicDependency _musicDependency;
        private CommandHandler _commandhandler;

        public botClient(DiscordSocketClient client = null, CommandService cmdservice = null)
        {
            //_client = client ?? new DiscordSocketClient(new DiscordSocketConfig { AlwaysDownloadUsers = true, //created a new discord socket client
            //    LogLevel = Discord.LogSeverity.Debug });

            //_commandsService = cmdservice ?? new CommandService(new CommandServiceConfig { LogLevel = Discord.LogSeverity.Debug,
            //    CaseSensitiveCommands = false }); //creates a new commandsservice, for support of [Command("name")] shit i think

            _serviceProvider = ConfigureServices();
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commandhandler = _serviceProvider.GetRequiredService<CommandHandler>();
            _node = _serviceProvider.GetRequiredService<LavaNode>();
        }


        public async Task RunAsync()
        {
            await _client.LoginAsync(Discord.TokenType.Bot, Values.getTokenTemp()); //logs into the bot
            await _client.StartAsync(); //starts the discord connection
            _client.Log += LogAsync; // log event for discord connection
            _musicDependency = new MusicDependency(_client, _serviceProvider, _node);
            _serviceProvider = ConfigureServices(); //create a serviceprovider for dependency injection
            /* _node = _serviceProvider.GetRequiredService<LavaNode>();*/ //gets the Lavanode from dependency injection

            await _musicDependency.InitAsync();

            await _commandhandler.RunAsync(); //dependency injection in the assembly

            await Task.Delay(-1); //stops it from quitting
        }

        private Task LogAsync(Discord.LogMessage arg)
        {
            Console.WriteLine(arg.Message);

            return Task.CompletedTask;
        }
        /* Configure our Services for Dependency Injection. */
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LavaNode>()
                .AddSingleton(new LavaConfig())
                .BuildServiceProvider();
        }
    }
}
