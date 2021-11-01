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

        public static void AddSong(string queuename, string songname)
        {
            List<string> temp = File.ReadAllLines(Path.Combine(path, queuename + ".txt")).ToList<string>();

            if (temp == null || temp.Count() == 0)
            {
                File.WriteAllText(Path.Combine(path, queuename + ".txt"), songname);
                return;
            }

            temp.Add(songname);
            File.WriteAllLines(Path.Combine(path, queuename + ".txt"), temp.ToArray());
        }


        private static void CheckFolder()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
