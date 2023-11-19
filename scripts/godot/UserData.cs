using Godot;

namespace OpenVoice
{
    public static class SessionManager
    {
        public static bool VerifyLogin(string Username, string Password)
        {
            if (Username == "") return false;
            foreach (string userFile in DirAccess.GetFilesAt("user:///users/"))
            {
                if (!userFile.EndsWith(".dat")) continue;
                string[] data = FileAccess.Open("user://users/" + userFile, FileAccess.ModeFlags.Read).GetAsText().Split('\n');

                if (Username == data[0] && Password == data[1]) { return true; }
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