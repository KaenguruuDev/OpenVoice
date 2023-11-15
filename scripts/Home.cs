using Godot;


namespace OpenVoice
{
	public partial class Home : Page
	{
		private UserData UserDataInstance;

		private Node2D CurrentWindowControls;

		public override void _Ready()
		{
			DisplayServer.WindowSetSize(new Vector2I(880, 495));
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, false);
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);

			PackedScene Controls = GD.Load<PackedScene>("res://scenes/pages/Home.tscn");
			CurrentWindowControls = Controls.Instantiate<Node2D>();
			AddChild(CurrentWindowControls);

			UserDataInstance = GetNode<UserData>("/root/UserData");
			UpdateTheme(UserDataInstance.GetTheme());

			GetNode<Button>("HomeControls/Buttons/VBoxContainer/AccountControls/LogIn").Pressed += ShowLoginPage;
			GetNode<Button>("HomeControls/Buttons/VBoxContainer/AccountControls/SignUp").Pressed += ShowSignUpPage;
		}


		protected override void UpdateTheme(Theme NewTheme)
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

		public void RequestPageChange(string ScenePath)
		{
			PackedScene Controls = GD.Load<PackedScene>(ScenePath);
			CurrentWindowControls.QueueFree();
			CurrentWindowControls = Controls.Instantiate<Node2D>();
			((Page) CurrentWindowControls).ForceUpdateTheme(UserDataInstance.GetTheme());
			AddChild(CurrentWindowControls);
		}

		private void ShowLoginPage()
		{
			RequestPageChange("res://scenes/pages/LogIn.tscn");
		}

		private void ShowSignUpPage()
		{
			RequestPageChange("res://scenes/pages/SignUp.tscn");
		}

		public UserData GetUserDataInstance()
		{ return UserDataInstance; }
	}
}