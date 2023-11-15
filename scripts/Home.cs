using Godot;


namespace OpenVoice
{
	public partial class Home : Node2D
	{
		private UserData UserDataInstance;

		private Node2D CurrentWindowControls;

		public override void _Ready()
		{
			DisplayServer.WindowSetSize(new Vector2I(880, 495));
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, false);
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);

			UserDataInstance = GetNode<UserData>("/root/UserData");
			UpdateTheme(UserDataInstance.GetTheme());

			GetNode<SignUp>("SignUp").UpdateTheme(UserDataInstance.GetTheme());
			GetNode<LogIn>("LogIn").UpdateTheme(UserDataInstance.GetTheme());
			UpdateTheme(UserDataInstance.GetTheme());

			GetNode<Button>("HomeControls/Buttons/VBoxContainer/AccountControls/LogIn").Pressed += ShowLogin;
			GetNode<Button>("HomeControls/Buttons/VBoxContainer/AccountControls/SignUp").Pressed += ShowSignUp;
		}


		public void UpdateTheme(Theme NewTheme)
		{
			RenderingServer.SetDefaultClearColor(NewTheme.GetColor(Theme.Palette.BACKGROUND));
			foreach (Control Ctrl in GetNode<VBoxContainer>("HomeControls/Buttons/VBoxContainer").GetChildren())
			{
				foreach (Button Btn in Ctrl.GetChildren())
				{
					Btn.AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT));
					Btn.AddThemeStyleboxOverride("hover", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY));
					Btn.AddThemeStyleboxOverride("pressed", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT));
					Btn.AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
				}
			}
		}
		
		public override void _Process(double delta)
		{

		}

		public void ShowLogin()
		{ GetNode<Node2D>("LogIn").Show(); GetNode<Node2D>("SignUp").Hide(); GetNode<Node2D>("HomeControls").Hide(); }
		public void ShowSignUp()
		{ GetNode<Node2D>("LogIn").Hide(); GetNode<Node2D>("SignUp").Show(); GetNode<Node2D>("HomeControls").Hide(); }
		public void ShowHome()
		{ GetNode<Node2D>("LogIn").Hide(); GetNode<Node2D>("SignUp").Hide(); GetNode<Node2D>("HomeControls").Show(); }

		public UserData GetUserDataInstance()
		{ return UserDataInstance; }
	}
}