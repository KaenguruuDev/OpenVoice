using Godot;
using System.Collections.Generic;

namespace OpenVoice
{
    public class Theme
    {
        public enum Palette
        {
            BACKGROUND,
            ACCENT,
            TEXT,
            PRIMARY,
            SECONDARY
        }

        private Color Background;
        private Color Primary;
        private Color Secondary;
        private Color Accent;
        private Color Text;

        private string Version;
        private string Name;
        private string Description;

        public Theme(string fPath)
        {
            if (FileAccess.FileExists(fPath) && fPath.EndsWith(".json"))
            {
                FileAccess ThemeFile = FileAccess.Open(fPath, FileAccess.ModeFlags.Read);
                Godot.Collections.Dictionary Data = Json.ParseString(ThemeFile.GetAsText()).AsGodotDictionary();
                Godot.Collections.Dictionary Colors = (Godot.Collections.Dictionary) Data.GetValueOrDefault("colors");
                
                Background = (Color) Colors.GetValueOrDefault("background");
                Primary = (Color) Colors.GetValueOrDefault("primary");
                Secondary = (Color) Colors.GetValueOrDefault("secondary");
                Accent = (Color) Colors.GetValueOrDefault("accent");
                Text = (Color) Colors.GetValueOrDefault("text");

                Version = (string) Data.GetValueOrDefault("version");
                Name = (string) Data.GetValueOrDefault("name");
                Description = (string) Data.GetValueOrDefault("description");

                ProjectSettings.SetSetting("display/window/size/borderless", true);
                ProjectSettings.SetSetting("display/window/size/resizable", false);
                RenderingServer.SetDefaultClearColor(Background);
            }
        }

        public Theme(Color BackGroundColor, Color PrimaryColor, Color SecondaryColor, Color AccentColor, Color TextColor)
        {

        }
    }
}