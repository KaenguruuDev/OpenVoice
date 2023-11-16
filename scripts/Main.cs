using Godot;
using System;

namespace OpenVoice
{
	public partial class Main : Node2D
	{
		public override void _Ready()
		{

		}

		

		public void UpdateTheme(Theme NewTheme)
		{
			foreach (Button Secondaries in GetNode<Node2D>("SecondaryBackgrounds").GetChildren())
			{
				Secondaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.SECONDARY));
			}
			foreach (Button Tertiaries in GetNode<Node2D>("TertiaryBackgrounds").GetChildren())
			{
				Tertiaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.BACKGROUND));
			}
		}
	}
}