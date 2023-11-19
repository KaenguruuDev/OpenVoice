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

        public User(string Username)
        {
            this.Username = Username;
        }

        public string GetUsername() { return Username; }
    }
}