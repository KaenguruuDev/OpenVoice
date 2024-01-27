using Godot;
using System;
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
                Godot.Collections.Dictionary Colors = (Godot.Collections.Dictionary)Data.GetValueOrDefault("colors");

                Background = (Color)Colors.GetValueOrDefault("background");
                Primary = (Color)Colors.GetValueOrDefault("primary");
                Secondary = (Color)Colors.GetValueOrDefault("secondary");
                Accent = (Color)Colors.GetValueOrDefault("accent");
                Text = (Color)Colors.GetValueOrDefault("text");

                Version = (string)Data.GetValueOrDefault("version");
                Name = (string)Data.GetValueOrDefault("name");
                Description = (string)Data.GetValueOrDefault("description");

                GD.Print(Text);
            }
        }

        public Theme(Color BackGroundColor, Color PrimaryColor, Color SecondaryColor, Color AccentColor, Color TextColor)
        {

        }

        public Color GetColor(Palette ColorType)
        {
            switch (ColorType)
            {
                case Palette.BACKGROUND:
                    return Background;
                case Palette.PRIMARY:
                    return Primary;
                case Palette.SECONDARY:
                    return Secondary;
                case Palette.TEXT:
                    return Text;
                case Palette.ACCENT:
                    return Accent;
                default:
                    return new Color("#ffffff");
            }

        }

        public enum StyleBoxType
        {
            EMPTY,
            FLAT,
            LINE
        }

        public enum StyleTarget
        {
            BUTTON,
            LINE_EDIT
        }

        public StyleBox GenerateStyleBoxFromTheme(StyleBoxType Type, StyleTarget Target, Palette Color, Vector4I? CornerRadius = null)
        {
            if (Type == StyleBoxType.EMPTY)
            { return new StyleBoxEmpty(); }

            if (Target == StyleTarget.BUTTON && Type == StyleBoxType.FLAT)
            {
                StyleBoxFlat NewStyleBox = new StyleBoxFlat { BgColor = GetColor(Color) };
                if (CornerRadius != null)
                {
                    NewStyleBox.CornerRadiusTopLeft = Math.Abs((int)CornerRadius?.X);
                    NewStyleBox.CornerRadiusTopRight = Math.Abs((int)CornerRadius?.Y);
                    NewStyleBox.CornerRadiusBottomLeft = Math.Abs((int)CornerRadius?.Z);
                    NewStyleBox.CornerRadiusBottomRight = Math.Abs((int)CornerRadius?.W);
                }
                return NewStyleBox;
            }

            if (Target == StyleTarget.LINE_EDIT && Type == StyleBoxType.FLAT)
            {
                StyleBoxFlat NewStyleBox = new StyleBoxFlat
                {
                    BgColor = GetColor(Color),
                    BorderColor = GetColor(Color)
                };
                NewStyleBox.SetCornerRadiusAll(5);
                NewStyleBox.BorderWidthTop = 4;
                NewStyleBox.BorderWidthLeft = 4;
                return NewStyleBox;
            }

            return new StyleBoxEmpty();
        }
    }
}