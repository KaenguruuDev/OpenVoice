using Newtonsoft.Json;
using System.Linq.Expressions;

namespace OpenVoice
{
    public class Theme
    {
        public enum Palette
        {
            BACKGROUND,
            TEXT,
            ACCENT,
            PRIMARY,
            SECONDARY
        }

        private string BackgroundColor = string.Empty;
        private string TextColor = string.Empty;
        private string AccentColor = string.Empty;
        private string PrimaryColor = string.Empty;
        private string SecondaryColor = string.Empty;

        private string version = string.Empty;
        private string name = string.Empty;
        private string description = string.Empty;

        public Theme(string BackgroundColor, string TextColor, string AccentColor, string PrimaryColor, string SecondaryColor, string version, string name, string description)
        {
            this.BackgroundColor = BackgroundColor;
            this.TextColor = TextColor;
            this.AccentColor = AccentColor;
            this.PrimaryColor = PrimaryColor;
            this.SecondaryColor = SecondaryColor;
            
            this.name = name;
            this.description = description;
            this.version = version;
        }

        // NOTE: Returns #ffffff if Colors has invalid data
        public string GetColor(Palette Color)
        {
            switch (Color)
            {
                case Palette.BACKGROUND:
                    return BackgroundColor;
                case Palette.TEXT:
                    return TextColor;
                case Palette.ACCENT:
                    return AccentColor;
                case Palette.PRIMARY:
                    return PrimaryColor;
                case Palette.SECONDARY:
                    return SecondaryColor;
                default:
                    return "#ffffff";
            }
        }
        public void SetColor(Palette Color, string Value, Signal ThemeChanged)
        {
            if (Color == Palette.BACKGROUND) BackgroundColor = Value;
            if (Color == Palette.TEXT) TextColor = Value;
            if (Color == Palette.ACCENT) AccentColor = Value;
            if (Color == Palette.PRIMARY) PrimaryColor = Value;
            if (Color == Palette.SECONDARY) SecondaryColor = Value;
            ThemeChanged.Emit();
        }

        public string GetName()
        { return name; }
        public string GetVersion()
        { return version; }
        public string GetDescription()
        { return description; }

    }

    public class ThemeControl
    {
        Theme? CurrentTheme = null;
        Signal ThemeChanged = new Signal("ThemeChanged");

        public ThemeControl()
        {
            // Default Colors
            CurrentTheme = new Theme("#211313", "#ffffff", "#d12600", "#ff6d4d", "#e6e6e5", "1.1", "Fallback", "Fallback Theme");
        }

        public void UpdateTheme()
        {
            ThemeChanged.Emit();
        }

        public void LoadTheme(string ThemePath)
        {
            if (File.Exists(ThemePath) && ThemePath.EndsWith(".json"))
            {
                try { CurrentTheme = JsonConvert.DeserializeObject<Theme>(File.ReadAllText(ThemePath)); }
                catch (FileLoadException e) { throw new FileLoadException("Unable to parse data from file '" + ThemePath + "': ", e); }
            } else throw new FileNotFoundException("File path/name is invalid!");
            ThemeChanged.Emit();
        }

        public void LoadTheme(Theme NewTheme)
        {
            if (NewTheme != null)
            {
                CurrentTheme = NewTheme;
            }
            else throw new ArgumentException("Theme cannot be null!");
            ThemeChanged.Emit();
        }

        public Theme? GetTheme()
        { return CurrentTheme; }
        public Signal? GetThemeChangedSignal()
        { return ThemeChanged; }
    }
}
