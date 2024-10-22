using Godot;

namespace OpenVoice
{
	public partial class SignUp : Node2D
	{
		private void SaveData(string Username, string Password)
		{
			string Data = Username + '\n' + Password;

			var f = FileAccess.OpenEncryptedWithPass(OS.GetUserDataDir() + "/users/" + Username + ".dat", FileAccess.ModeFlags.Write, DataSecurity.GetEncryptionKey()); f.StoreString(Data); f.Close();
		}

		public override void _Ready()
		{
			GetNode<Button>("Buttons/VBoxContainer/Other/Confirm").Pressed += ConfirmSignUp;
			GetNode<Button>("Buttons/VBoxContainer/Other/Cancel").Pressed += ReturnToHome;
		}

		public override void _Process(double delta)
		{

		}

		public void UpdateTheme(Theme NewTheme)
		{
			RenderingServer.SetDefaultClearColor(NewTheme.GetColor(Theme.Palette.BACKGROUND));
			foreach (Button Btn in GetNode<HBoxContainer>("Buttons/VBoxContainer/Other").GetChildren())
			{
				Btn.AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT, new Vector4I(8, 8, 8, 8)));
				Btn.AddThemeStyleboxOverride("hover", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
				Btn.AddThemeStyleboxOverride("pressed", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT, new Vector4I(8, 8, 8, 8)));
				Btn.AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
			}
			GetNode<LineEdit>("Username").AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.SECONDARY));
			GetNode<LineEdit>("Password").AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.SECONDARY));
			GetNode<LineEdit>("ConfirmPassword").AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.SECONDARY));
			GetNode<LineEdit>("Username").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
			GetNode<LineEdit>("Password").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
			GetNode<LineEdit>("ConfirmPassword").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
			GetNode<Label>("WarningPasswordTooWeak").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.ACCENT));
			GetNode<Label>("WarningPasswordsNotMatching").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.ACCENT));
		}

		private void ReturnToHome() { GetParent<WindowController>().ShowHome(); }

		private void ConfirmSignUp()
		{
			if (GetNode<LineEdit>("Password").Text == GetNode<LineEdit>("ConfirmPassword").Text && GetNode<LineEdit>("Username").Text != "")
			{
				SaveData(GetNode<LineEdit>("Username").Text, GetNode<LineEdit>("Password").Text);
				ReturnToHome();
			}
		}
	}
}