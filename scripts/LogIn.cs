using System.Security;
using Godot;

namespace OpenVoice
{
	public partial class LogIn : Node2D
	{
		private string UsernameInput;

		public override void _Ready()
		{
			GetNode<Button>("Buttons/VBoxContainer/Other/Confirm").Pressed += ConfirmLogin;
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
		}

		public void UsernameInputChanged(string NewName)
		{
			UsernameInput = NewName;
		}

		private void ReturnToHome() { GetParent<Home>().ShowHome(); }

		private void ConfirmLogin() { PasswordSubmitted(GetNode<LineEdit>("Username").Text); }

		public void PasswordSubmitted(string Password)
		{
			if (SessionManager.VerifyLogin(UsernameInput, GenerateSecureFromStr(Password)))
			{ GD.Print("LOGIN SUCCESS!"); }
		}

		private SecureString GenerateSecureFromStr(string Str)
        {
            SecureString SecStr = new SecureString();
            for (int i = 0; i < Str.Length; i++)
            {
                SecStr.AppendChar(Str[i]);
            }
            return SecStr;
        }
	}
}