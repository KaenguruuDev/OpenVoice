using Godot;

#nullable enable
namespace OpenVoice
{
    public class User
    {
        private enum Mode
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
    }
}