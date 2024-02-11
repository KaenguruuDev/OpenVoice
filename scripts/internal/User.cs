using Godot;

#nullable enable
namespace OpenVoice
{
    public class User
    {
        public enum Mode
        {
            ONLINE,
            OFFLINE,
            NO_DISTURB,
            AWAY
        }

        private string Username;
        private Image Avatar = new Image();
        private string Status = string.Empty;
        private Mode UserMode = Mode.ONLINE;


        private int UUID;
        private int Privileges;

        public User(string Username, int Privileges = -1, int UUID = -1)
        {
            this.Username = Username;
            if (UUID != -1) this.UUID = UUID;
            if (UUID != -1) this.Privileges = Privileges;
        }

        public string GetUsername() { return Username; }
        public int GetUUID() { return UUID; }
        public int GetPrivileges() { return Privileges; }
        public Image GetAvatar() { return Avatar; }
        public string GetStatus() { return Status; }
        public Mode GetUserMode() { return UserMode; }

        public void SetAvatar(Image Avatar) { this.Avatar = Avatar; }
        public void SetStatus(string Status) { this.Status = Status; }
        public void SetUserMode(Mode UserMode) { this.UserMode = UserMode; }

    }
}