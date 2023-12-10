using System.IO;
using Godot;


namespace OpenVoice
{
	public partial class WindowController : Node2D
	{
		private Node2D CurrentWindowControls;

		public override void _Ready()
		{
			Directory.CreateDirectory(OS.GetUserDataDir() + "/users");
			Directory.CreateDirectory(OS.GetUserDataDir() + "/sessions");

			if (SessionManager.ValidSessionAvailable()) { ShowMain(); }

			DisplayServer.WindowSetSize(new Vector2I(880, 495));
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, false);
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);

			var screen_size = (Vector2) DisplayServer.ScreenGetSize(1);
  			DisplayServer.WindowSetPosition((Vector2I) (screen_size * 0.5f - new Vector2(880f, 495f) * 0.5f) + new Vector2I(1920, 0));

			UpdateTheme(UserData.GetTheme());

			GetNode<SignUp>("SignUp").UpdateTheme(UserData.GetTheme());
			GetNode<LogIn>("LogIn").UpdateTheme(UserData.GetTheme());
			GetNode<Main>("Main").UpdateTheme(UserData.GetTheme());
			UpdateTheme(UserData.GetTheme());

			GetNode<Button>("HomeControls/Buttons/VBoxContainer/AccountControls/LogIn").Pressed += ShowLogin;
			GetNode<Button>("HomeControls/Buttons/VBoxContainer/AccountControls/SignUp").Pressed += ShowSignUp;

			GetNode<Button>("HomeControls/Buttons/VBoxContainer/Other/Exit").Pressed += ExitApplication;
		}


		public void UpdateTheme(Theme NewTheme)
		{
			RenderingServer.SetDefaultClearColor(NewTheme.GetColor(Theme.Palette.BACKGROUND));
			foreach (Control Ctrl in GetNode<VBoxContainer>("HomeControls/Buttons/VBoxContainer").GetChildren())
			{
				foreach (Button Btn in Ctrl.GetChildren())
				{
					Btn.AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT, new Vector4I(8, 8, 8, 8)));
					Btn.AddThemeStyleboxOverride("hover", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
					Btn.AddThemeStyleboxOverride("pressed", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT, new Vector4I(8, 8, 8, 8)));
					Btn.AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
				}
			}
		}

		public void ShowLogin()
		{ GetNode<Node2D>("LogIn").Show(); GetNode<Node2D>("SignUp").Hide(); GetNode<Node2D>("HomeControls").Hide(); GetNode<Node2D>("Main").Hide(); }
		public void ShowSignUp()
		{ GetNode<Node2D>("LogIn").Hide(); GetNode<Node2D>("SignUp").Show(); GetNode<Node2D>("HomeControls").Hide(); GetNode<Node2D>("Main").Hide(); }
		public void ShowHome()
		{ GetNode<Node2D>("LogIn").Hide(); GetNode<Node2D>("SignUp").Hide(); GetNode<Node2D>("HomeControls").Show(); GetNode<Node2D>("Main").Hide(); }
		public void ShowMain()
		{ GetNode<Node2D>("LogIn").Hide(); GetNode<Node2D>("SignUp").Hide(); GetNode<Node2D>("HomeControls").Hide(); GetNode<Node2D>("Main").Show(); }

		public void ExitApplication()
		{ GetTree().Quit(); }
	}
}