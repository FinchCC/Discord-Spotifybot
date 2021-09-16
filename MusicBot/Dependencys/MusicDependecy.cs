using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace MusicBot.Dependencys
{
    public class MusicDependency
    {
        private LavaNode _lavaNode;
        private LavaSocket _lavaSocket;
        private readonly DiscordSocketClient _client;
        private readonly ServiceProvider _service;

        public MusicDependency(DiscordSocketClient socketClient, ServiceProvider service, LavaNode node)
        {
            _client = socketClient;
            _service = service;
            _lavaNode = node;
        }

        public Task InitAsync()
        {
            _client.Ready += client_readyAsync;
            _lavaNode.ConnectAsync();
            _lavaNode.OnLog += _lavaNode_OnLog;
            _lavaNode.OnTrackEnded += _lavaNode_OnTrackEnded;

            return Task.CompletedTask;
        }       
        
        private async Task client_readyAsync()
        {
            await _lavaNode.ConnectAsync();
        }

        private async Task _lavaNode_OnTrackEnded(Victoria.EventArgs.TrackEndedEventArgs arg)
        {
            if (arg.Reason == Victoria.Enums.TrackEndReason.Stopped)
                return;

            //if (Values.repeat == true)
            //{
            //    await arg.Player.StopAsync(); not ready yet
            //    await arg.Player.PlayAsync(arg.Player.VoiceChannel.);
            //}

            if (!arg.Player.Queue.TryDequeue(out var queueable))
                return;



            if(!(queueable is LavaTrack track))
            {
                await arg.Player.TextChannel.SendMessageAsync("Not a valid song");
                return;
            }

            await arg.Player.StopAsync();
            await arg.Player.PlayAsync(track);
        }

        private Task _lavaNode_OnLog(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }



       
    }
}
