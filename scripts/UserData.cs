using Godot;
using System;
using System.Collections.Generic;

namespace OpenVoice
{
    [Serializable]
    public partial class User : GodotObject
    {
        private string Username;
        private string Password;
        private DateTime SessionTime;

        public string GetUsername() { return Username; }
        public string GetPassword() { return Password; }
        public DateTime GetSessionTime() { return SessionTime; }

        public User(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }
    }

    public class SessionManager
    {
        public static bool VerifyLogin(string Username, string Password)
        {
            if (Username == "") return false;
            foreach (string userFile in DirAccess.GetFilesAt("user:///users/"))
            {
                if (!userFile.EndsWith(".dat")) continue;
                string[] data = FileAccess.Open("user://users/" + userFile, FileAccess.ModeFlags.Read).GetAsText().Split('\n');
                
                User LoadedUser = new User(data[0], data[1]);
                if (Username == LoadedUser.GetUsername() && Password == LoadedUser.GetPassword()) { return true; }
            }

            return false;
        }
    }

    public partial class UserData : Node
    {
        private Theme CurrentTheme;

        public override void _Ready()
        {
            LoadData();
        }

        public void LoadData()
        {
            LoadSettings();
            CurrentTheme = new Theme("assets/themes/blue_skies.json");
            GetNode<Launch>("/root/Launch").SetUserDataInstance(this);
        }

        public bool LoadSettings()
        {
            return true;
        }

        public bool TryResumeSession()
        {
            return true;
        }

        public Theme GetTheme() { return CurrentTheme; }
    }
}