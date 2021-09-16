using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdservice;
        private readonly IServiceProvider _services;


        public CommandHandler(IServiceProvider service)
        {
            _cmdservice = service.GetRequiredService<CommandService>();
            _client = service.GetRequiredService<DiscordSocketClient>();
            _services = service;
            HookEvents();
        }

        public async Task RunAsync()
        {
            await _cmdservice.AddModulesAsync(Assembly.GetEntryAssembly(), _services); //injects the service provider inside the assemblycode
            // (assemblycodes is just code in bytform i think) -> i guess this makes the search for commands trough diffrent classes possible          
        }

        public void HookEvents()
        {
            _cmdservice.Log += _cmdservice_Log; //log event
            _client.MessageReceived += HandleMsgRecives; //gets triggered every time a message is sent
        }

        private async Task HandleMsgRecives(SocketMessage arg)
        {
            int pos = 0;

            if (arg.Author.IsBot) return;
            if (!((SocketUserMessage)arg).HasCharPrefix(Values.prefix, ref pos)) return;

            var userMessages = arg as SocketUserMessage; //message class that contains the information about message and shit


            var context = new SocketCommandContext(_client, userMessages); //represent a class with the context of the sender
            var result = await _cmdservice.ExecuteAsync(context, pos, _services); //executes the command

            Console.WriteLine("message recived");

            return;
        }

        private Task _cmdservice_Log(Discord.LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}
