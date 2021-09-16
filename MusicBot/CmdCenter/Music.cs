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
        public static readonly string arrow = ":arrow_forward:";
        public static readonly string cross = ":red_square:";

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
                await ReplyAsync("**Joined voicechannel: **" + voicechatname);
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


            var songs = queue.Split('/');
            foreach (var song in songs)
            {
                var searchResponse = await _lavaNode.SearchYouTubeAsync(song);
                if (searchResponse.Status == Victoria.Responses.Search.SearchStatus.NoMatches ||
                    searchResponse.Status == Victoria.Responses.Search.SearchStatus.LoadFailed)
                {
                    await ReplyAsync($"{cross}Could not find any songs on youtube for **{songs}**");
                    return;
                }


                var person = _lavaNode.GetPlayer(Context.Guild);
                if (person.PlayerState == Victoria.Enums.PlayerState.Playing || person.PlayerState == Victoria.Enums.PlayerState.Paused)
                {
                    if (!string.IsNullOrEmpty(searchResponse.Playlist.Name))
                    {
                        foreach (var track in searchResponse.Tracks)
                            person.Queue.Enqueue(track);

                        await ReplyAsync($"{arrow}Enqueued {searchResponse.Tracks.Count}");
                    }
                    else
                    {
                        var track = searchResponse.Tracks.ElementAt(0);
                        person.Queue.Enqueue(track);
                        await ReplyAsync($"{arrow}Queued: **{track.Title} - {track.Author}**");
                    }
                }
                else
                {
                    var track = searchResponse.Tracks.ElementAt(0);

                    if (!string.IsNullOrEmpty(searchResponse.Playlist.Name))
                    {
                        await person.PlayAsync(track);
                        await ReplyAsync($"{arrow}Now playing **{track.Title}**");
                        if (searchResponse.Tracks.Count > 1)
                        {
                            for (int i = 1; i < searchResponse.Tracks.Count; i++)
                            {
                                person.Queue.Enqueue(searchResponse.Tracks.ElementAt(i));

                            }
                            await ReplyAsync($"{arrow}Enqueued **{searchResponse.Tracks.Count - 1}** songs");
                        }
                    }
                    else
                    {
                        await person.PlayAsync(track);
                        await ReplyAsync($"{arrow}Now playing **{track.Title}**");
                    }
                }


            }

        }

        [Command("Splay"), Alias("spl")]
        public async Task spoityplay([Remainder] string plid)
        {
            if(string.IsNullOrEmpty(plid))
            {
                await ReplyAsync("Provide a playlistid");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild)) //lavanode i guess refreer to the connection between lavanode and a person in voice
            {
                await ReplyAsync("Connecting to a voice chat");
                try
                {
                    await join();
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{cross}Could not connect to a voice chat, error: {e.Message}");
                    return;
                }
            }

            var person = _lavaNode.GetPlayer(Context.Guild);

            string[] songnames = await SpotifyParser.getSongNamesArr(plid);
            if(songnames == null|| songnames.Count() == 0)
            {
                await ReplyAsync($"{cross}Wrong or no playlist id");
                return;
            }

            int totalqueued = 0;

            foreach (var songs in songnames)
            {
                var searchResponse = await _lavaNode.SearchYouTubeAsync(songs);
                if (searchResponse.Status == Victoria.Responses.Search.SearchStatus.NoMatches ||
                    searchResponse.Status == Victoria.Responses.Search.SearchStatus.LoadFailed)
                {
                    await ReplyAsync($"Could not find any songs on youtube for **{songs}**");
                    continue;
                }

                if(person.PlayerState == Victoria.Enums.PlayerState.Playing || person.PlayerState == Victoria.Enums.PlayerState.Paused)
                {
                    try
                    {
                        person.Queue.Enqueue(searchResponse.Tracks.ElementAt(0));
                        totalqueued++;
                        Console.WriteLine($"{arrow}Enqueued {songs}");
                    }
                    catch (Exception e)
                    {
                        await ReplyAsync($"{cross}Could not queue {songs}, Error: {e.Message}");
                        continue;
                    }                    
                }
                else
                {
                    var track = searchResponse.Tracks.ElementAt(0);
                    try
                    {
                        await person.PlayAsync(track);
                    }
                    catch(Exception e)
                    {
                        await ReplyAsync($"{cross}Could not queue {songs}, Error: {e.Message}");
                        continue;
                    }
                }
            }

            await ReplyAsync($"{arrow}Enqueued **{totalqueued}** songs");
        }

        [Command("Pause"), Alias("ps")]
        public async Task pause()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var person) || person.PlayerState != Victoria.Enums.PlayerState.Playing)
            {
                await ReplyAsync($"{cross}Not even playing a song mongo");
                return;
            }
            else
            {
                try
                {
                    await person.PauseAsync();
                    await ReplyAsync($"{arrow}Paused {person.Track.Title}");
                    return;
                }
                catch (Exception e)
                {

                    await ReplyAsync($"{cross}Error: {e.Message}");
                }
            }
        }

        [Command("Resume"), Alias("r")]
        public async Task resume()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var person) || person.PlayerState != Victoria.Enums.PlayerState.Paused)
            {
                await ReplyAsync($"{cross}Either already playing or not connected to a voicechannel");
                return;
            }
            else
            {
                try
                {
                    await person.ResumeAsync();
                    await ReplyAsync($"{arrow}Resumed playing **{person.Track.Title}**");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{cross}Error: {e.Message}");
                }
            }

        }

        [Command("Skip"), Alias("s")]
        public async Task skip()
        {
            if (_lavaNode.TryGetPlayer(Context.Guild, out var person) == false)
            {
                await ReplyAsync($"{cross}Either already playing or not connected to a voicechannel");
                return;
            }
            else
            {
                try
                {
                    var user = Context.User.Mention;
                    var previossong = person.Track;
                    await person.SkipAsync();
                    await ReplyAsync($"{arrow}{user} skipped **{previossong.Title} - {previossong.Author}**");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{cross}Error skipping: {e.Message}");
                }
            }

        }

        [Command("nowplaying"), Alias("np")]
        public async Task nowplaying()
        {
            var person = _lavaNode.GetPlayer(Context.Guild);
            if(person.PlayerState == Victoria.Enums.PlayerState.Stopped || person.PlayerState == Victoria.Enums.PlayerState.None)
            {
                await ReplyAsync("Not even playing");
                return;
            }
            
            await ReplyAsync($"{arrow}Currently playing: **{person.Track.Title} - {person.Track.Author}**");
        }


        [Command("queue"), Alias("q")]
        public async Task checkqueue()
        {
            var person = _lavaNode.GetPlayer(Context.Guild);
            if (person.PlayerState == Victoria.Enums.PlayerState.Stopped || person.PlayerState == Victoria.Enums.PlayerState.None)
            {
                await ReplyAsync("Not even playing");
                return;
            }
            if (person.Queue.Count() < 1 && person.Track != null)
            {
                await ReplyAsync($"{arrow}Currently playing: **{person.Track.Title}**, no other sounds queued");
            }
            else
            {
                string queuelist = $"{arrow} **Queue List:**" +
                    $"\nCurrently playing: **{person.Track.Title}**\n";
                int count = 0;
                foreach (LavaTrack track in person.Queue)
                {
                    if (count > 5)
                        break;
                    queuelist = queuelist + "Song number " + (count + 1) + ": **" + track.Title + " - " + track.Author + "**\n";
                    count++;
                }
                await ReplyAsync(queuelist);
                return;
            }
        }

        [Command("source")]
        public async Task source()
        {
            await ReplyAsync($"{arrow}Sourcecode ---> https://github.com/FinchCC/SpotifyApi-Musicbot");
        }

        [Command("repeat"), Alias("r")]
        public async Task repeat()
        {
            Values.repeat = !Values.repeat;
            await ReplyAsync($"{arrow}Repeat is now set to: {Values.repeat.ToString()}");
        }


        [Command("Volume"), Alias("v")]
        public async Task SetVolume(int ammount)
        {
            if(ammount > 150 || ammount <= 0)
            {
                await ReplyAsync($"{cross}**Volume must be between 0 and 150*");
                return;
            }
            try
            {
                var person = _lavaNode.GetPlayer(Context.Guild);
                await person.UpdateVolumeAsync((ushort)ammount);
                await ReplyAsync($"{arrow}**New volume is set to {ammount.ToString()}**");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        [Command("Volume"), Alias("v")]
        public async Task SetVolume()
        {
            var person = _lavaNode.GetPlayer(Context.Guild);
            try
            {
                await ReplyAsync($"{arrow}**Volume is currently set to:** {person.Volume.ToString()}");
            }
            catch
            {
                await ReplyAsync($"{cross}**Could not get volume**");
            }
        }

        [Command("replay"), Alias("rp")]
        public async Task replay()
        {
            var person = _lavaNode.GetPlayer(Context.Guild);
            if(person.PlayerState != Victoria.Enums.PlayerState.Playing)
            {
                await ReplyAsync($"{cross}**Play something first**");
                return;
            }

            var track = person.Track;

            try
            {
                await person.PlayAsync(track);
                await ReplyAsync($"Replaying: **{track.Title} - {track.Author}**");                
            }
            catch(Exception e)
            {
                await ReplyAsync($"{cross}**Could not replay the current song, Error: {e.Message}**");
            }
        }

    }
}

