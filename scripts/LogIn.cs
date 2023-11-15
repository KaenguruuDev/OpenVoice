using CredentialManagement;
using System.Security;
using Godot;
using System.Runtime.CompilerServices;

namespace OpenVoice
{
	public partial class LogIn : Page
	{
		private string UsernameInput;

		public override void _Ready()
		{
			UpdateTheme(GetParent<Home>().GetUserDataInstance().GetTheme());
		}

        public override void _Process(double delta)
		{

		}

		protected override void UpdateTheme(Theme NewTheme)
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