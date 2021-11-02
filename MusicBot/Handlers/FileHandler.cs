using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Handlers
{
    public class FileHandler
    {
        private static string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Musicbot_Lists");
        public static string CreateQueue(string name)
        {
            CheckFolder();

            if (File.Exists(Path.Combine(path, name + ".txt")))
                return (MusicBot.CmdCenter.Music.cross + "**Saved Queue with that name already exists**");
            else
            {
                File.Create(Path.Combine(path, name + ".txt"));
                return (CmdCenter.Music.arrow + "**Created a new saved Queue with the name: " + name + "**");
            }
        }

        public static bool CheckQueue(string name)
        {
            if (File.Exists(Path.Combine(path, name + ".txt")))
                return true;
            else
                return false;
        }

        public static string AddSong(string queuename, string songname)
        {
            List<string> temp = File.ReadAllLines(Path.Combine(path, queuename + ".txt")).ToList<string>();

            if (temp == null || temp.Count() == 0)
            {
                File.WriteAllText(Path.Combine(path, queuename + ".txt"), songname);
                return ("**Added: " + songname + "to: " + queuename + "**");
            }

            temp.Add(songname);
            File.WriteAllLines(Path.Combine(path, queuename + ".txt"), temp.ToArray());

            return ("**Added: " + songname + "to: " + queuename + "**");
        }

        public static string getSongs(string queuename)
        {
            if (!CheckQueue(queuename))
                return "**No queue with that name**";

            var temp = rFile(queuename);
            string songs = string.Join("\n", temp);
            int count = temp.Count();

            return ("**" + queuename + "** Contains **" + count + "**:" + "\n" + "*" + songs + "*");

        }

        public static string[] rFile(string filename)
        {
            return File.ReadAllLines(Path.Combine(path, filename + ".txt"));
        }

        private static int countLines(string filename)
        {
            return rFile(filename).Count();
        }

        public static string queueList()
        {
            var files = Directory.GetFiles(path).Select(file => Path.GetFileName(file)).Select(name => name.Replace(".txt", "")).ToArray();
            int lcount = files.Count();

            if (lcount == 0)
                return "**No queues saved, try creating one with:** -newq 'name'";
            
            string message = "**Queue saved:**";            
            foreach (var list in files)
            {
                message += ("\n**" + list + "** Contains **" + countLines(list).ToString() + "** songs");
            }

            return message;
        }

        public static string deleteList(string name)
        {
            if (!CheckQueue(name))
                return "**No queues saved, try creating one with:** -newq 'name'";

            File.Delete(Path.Combine(path, name + ".txt"));

            return ("**" + name + "** is now **deleted**");
        }


        private static void CheckFolder()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
