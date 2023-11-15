using Godot;

namespace OpenVoice
{
    public partial class Page : Node2D
    {
        public void ForceUpdateTheme(Theme NewTheme)
        {
            UpdateTheme(NewTheme);
        }

        protected virtual void UpdateTheme(Theme NewTheme)
        { }
    }
}