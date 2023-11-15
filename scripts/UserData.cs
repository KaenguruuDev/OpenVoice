using Godot;


namespace OpenVoice
{
    public partial class UserData : Node
    {
        public Signal DataLoaded;
        private Theme CurrentTheme;

        public override void _Ready()
        {
            DataLoaded = new Signal(this, "UserData.Loaded");
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
    }
}