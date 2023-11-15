using Godot;
using CredentialManagement;

namespace OpenVoice
{
	public partial class SignUp : Node2D
	{
		private void SaveData(string Username, string Password)
		{
			using (var cred = new Credential())
			{
				cred.Password = Password;
				cred.Target = Username;
				cred.Type = CredentialType.Generic;
				cred.PersistanceType = PersistanceType.LocalComputer;
				cred.Save();
			}
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
				Btn.AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT));
				Btn.AddThemeStyleboxOverride("hover", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY));
				Btn.AddThemeStyleboxOverride("pressed", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT));
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

		private void ReturnToHome() { GetParent<Home>().ShowHome(); }

		private void ConfirmSignUp()
		{
			
		}
	}
}