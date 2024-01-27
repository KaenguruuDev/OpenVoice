using Godot;
using Godot.Collections;

using System;
using System.Linq;
using System.Text;

namespace OpenVoice
{
    public static class SessionManager
    {
        public static bool ValidSessionAvailable()
        {
            if (!FileAccess.FileExists("user://sessions/latest.dat")) return false;

            Variant Latest = FileAccess.Open("user://sessions/latest.dat", FileAccess.ModeFlags.Read).GetVar();
            string LatestUser = Latest.AsGodotDictionary().Values.ToArray()[0].ToString();

            Variant SessionData = FileAccess.Open("user://sessions/" + LatestUser + ".dat", FileAccess.ModeFlags.Read).GetVar();
            string user = (string)SessionData.AsGodotDictionary().Values.ToArray()[0];
            string token = (string)SessionData.AsGodotDictionary().Values.ToArray()[1];

            byte[] data = Convert.FromBase64String(user);
            string DecodedUser = Encoding.UTF8.GetString(data);

            data = Convert.FromBase64String(token);
            string DecodedToken = Encoding.UTF8.GetString(data);

            if (DecodedUser == LatestUser && DateTime.Now.ToUniversalTime().Ticks - DateTime.FromFileTime((long)Convert.ToDouble(DecodedToken)).ToUniversalTime().Ticks < 9000)
            { return true; }

            return false;
        }

        public static bool VerifyLogin(string Username, string Password)
        {
            if (Username == "") return false;
            foreach (string userFile in DirAccess.GetFilesAt("user://users/"))
            {
                if (!userFile.EndsWith(".dat")) continue;
                string[] data = FileAccess.Open("user://users/" + userFile, FileAccess.ModeFlags.Read).GetAsText().Split('\n');

                if (Username == data[0] && Password == data[1])
                {
                    Dictionary<string, string> SessionData = new Dictionary<string, string>
                    {
                        { "user", Convert.ToBase64String(Encoding.UTF8.GetBytes(Username)) },
                        { "token", Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToUniversalTime().Ticks.ToString())) }
                    };

                    FileAccess.Open("user://sessions/" + Username + ".dat", FileAccess.ModeFlags.Write).StoreVar(SessionData);

                    Dictionary<string, string> Latest = new Dictionary<string, string>
                    {
                        { "last_user", Username }
                    };

                    FileAccess.Open("user://sessions/latest.dat", FileAccess.ModeFlags.Write).StoreVar(Latest);

                    return true;
                }
            }

            return false;
        }
    }

    public static class UserData
    {
        private static Theme CurrentTheme;

        public static void LoadData()
        {
            LoadSettings();
            CurrentTheme = new Theme("assets/themes/blue_skies.json");
        }

        public static bool LoadSettings()
        {
            return true;
        }

        public static bool TryResumeSession()
        {
            return true;
        }

        public static Theme GetTheme() { return CurrentTheme; }
    }
}