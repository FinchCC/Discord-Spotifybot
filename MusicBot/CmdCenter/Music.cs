using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace MusicBot.CmdCenter
{
    public class Music : ModuleBase<SocketCommandContext>
    {

        private readonly LavaNode _lavaNode;

        public Music(LavaNode node)
            => _lavaNode = node;


        /// <summary>
        /// If a user is in a voice chat, and the bot is not connected to any of the voicechats on the guild
        ///  --->it will join
        /// </summary>
        /// <returns>returns null</returns>
        /// 
        [Command("ping")]
        public async Task ping()
        {
            await ReplyAsync("bong");
        }
        [Command("Join")]
        public async Task join()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Already in a voicechat");
                return;
            }

            var voicestate = Context.User as IVoiceState; // the voicestatus of the context(message) sender(user)
            if (voicestate.VoiceChannel == null)
            {
                await ReplyAsync("Join a voicechat");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voicestate.VoiceChannel); //join the specified voicechannel, no idea why it can join textchannel as a second
                //parameter
                string voicechatname = voicestate.VoiceChannel.Name;
                await ReplyAsync("joined #" + voicechatname);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("Leave")]
        public async Task leave()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    var voicestate = Context.User as IVoiceState;
                    await _lavaNode.LeaveAsync(voicestate.VoiceChannel);
                    await ReplyAsync("Left voicechat");
                }
                catch (Exception e)
                {
                    await ReplyAsync(e.Message);
                }
                return;
            }
            else
                await ReplyAsync("Not even in a voicechat");
            return;
        }


        [Command("Play"), Alias("p")]
        public async Task play([Remainder] string queue)
        {
            if (string.IsNullOrEmpty(queue))
            {
                await ReplyAsync("Provide a song");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild)) //lavanode i guess refreer to the connection between lavanode and a person in voice
            {
                await ReplyAsync("Connecting to a voice chat");
                try
                {
                    await join();
                }
                catch(Exception e)
                {
                    await ReplyAsync($"Could not connect to a voice chat, error: {e.Message}");
                    return;
                }
            }


            var songs = queue.Split(' ');
            foreach (var song in songs)
            {
                var searchResponse = await _lavaNode.SearchYouTubeAsync(song);
                if (searchResponse.Status == Victoria.Responses.Search.SearchStatus.NoMatches ||
                    searchResponse.Status == Victoria.Responses.Search.SearchStatus.LoadFailed)
                {
                    await ReplyAsync($"Could not find any songs on youtube for **{songs}**");
                    return;
                }


                var person = _lavaNode.GetPlayer(Context.Guild);
                if (person.PlayerState == Victoria.Enums.PlayerState.Playing || person.PlayerState == Victoria.Enums.PlayerState.Paused)
                {
                    if (!string.IsNullOrEmpty(searchResponse.Playlist.Name))
                    {
                        foreach (var track in searchResponse.Tracks)
                            person.Queue.Enqueue(track);

                        await ReplyAsync($"Enqueued {searchResponse.Tracks.Count}");
                    }
                    else
                    {
                        var track = searchResponse.Tracks.ElementAt(0);
                        person.Queue.Enqueue(track);
                        await ReplyAsync($"Queued {track.Title}");
                    }
                }
                else
                {
                    var track = searchResponse.Tracks.ElementAt(0);

                    if (!string.IsNullOrEmpty(searchResponse.Playlist.Name))
                    {
                        await person.PlayAsync(track);
                        await ReplyAsync($"Now playing {track.Title}");
                        if (searchResponse.Tracks.Count > 1)
                        {
                            for (int i = 1; i < searchResponse.Tracks.Count; i++)
                            {
                                person.Queue.Enqueue(searchResponse.Tracks.ElementAt(i));

                            }
                            await ReplyAsync($"Enqueued {searchResponse.Tracks.Count - 1} songs");
                        }
                    }
                    else
                    {
                        await person.PlayAsync(track);
                        await ReplyAsync($"Now playing {track.Title}");
                    }
                }


            }

        }

        [Command("Splay"), Alias("spl")]
        public async Task spoityplay([Remainder] string plid)
        {
            if (!_lavaNode.HasPlayer(Context.Guild)) //lavanode i guess refreer to the connection between lavanode and a person in voice
            {
                await ReplyAsync("Connect to a voice chat");
                return;
            }
        }

        [Command("Pause"), Alias("ps")]
        public async Task pause()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var person) || person.PlayerState != Victoria.Enums.PlayerState.Playing)
            {
                await ReplyAsync("Not even playing a song mongo");
                return;
            }
            else
            {
                try
                {
                    await person.PauseAsync();
                    await ReplyAsync($"Paused {person.Track.Title}");
                    return;
                }
                catch (Exception e)
                {

                    await ReplyAsync($"Error: {e.Message}");
                }
            }
        }

        [Command("Resume"), Alias("r")]
        public async Task resume()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var person) || person.PlayerState != Victoria.Enums.PlayerState.Paused)
            {
                await ReplyAsync("Either already playing or not connected to a voicechannel");
                return;
            }
            else
            {
                try
                {
                    await person.ResumeAsync();
                    await ReplyAsync($"Resumed playing **{person.Track.Title}**");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Error: {e.Message}");
                }
            }

        }

        [Command("Skip"), Alias("s")]
        public async Task skip()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var person) || person.PlayerState != Victoria.Enums.PlayerState.Paused)
            {
                await ReplyAsync("Either already playing or not connected to a voicechannel");
                return;
            }
            else
            {
                try
                {
                    var user = Context.User.Mention;
                    var previossong = person.Track.Title;
                    await person.SkipAsync();
                    await ReplyAsync($"{user} skipped {previossong}");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Error skipping: {e.Message}");
                }
            }

        }
    }
}

