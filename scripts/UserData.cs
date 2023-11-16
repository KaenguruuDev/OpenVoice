using Godot;
using System;
using System.IO;

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

        public User(string Username, string Password, DateTime SessionTime)
        {
            this.Username = Username;
            this.Password = Password;
            this.SessionTime = SessionTime;
        }
    }

    public class SessionManager
    {
        public static bool VerifyLogin(string Username, string Password)
        {
            if (Username == "") return false;
            foreach (string userFile in Directory.GetFiles("user://users/"))
            {
                User User = (User) Godot.FileAccess.Open(userFile, Godot.FileAccess.ModeFlags.Read).GetVar().AsGodotObject();
                if (Username == User.GetUsername() && Password == User.GetPassword()) { return true; }
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