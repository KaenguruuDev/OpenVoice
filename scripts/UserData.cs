using Godot;
using System;
using System.Collections.Generic;

namespace OpenVoice
{
    public class User
    {
        private string Username;

        public string GetUsername() { return Username; }

        public User(string Username)
        {
            this.Username = Username;
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

                if (Username == data[0] && Password == data[1]) { return true; }
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