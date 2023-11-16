using System.IO;
using Godot;

namespace OpenVoice
{
	public partial class Launch : Node2D
	{
		private UserData UserDataInstance;

		private async void LoadData()
		{
			Directory.CreateDirectory(OS.GetUserDataDir() + "/users");

			while (UserDataInstance == null) { }
			GetTree().Root.CallDeferred("add_child", GD.Load<PackedScene>("WindowController.tscn").Instantiate());
			QueueFree();
		}

		public override void _Ready()
		{
			LoadData();
		}

		public override void _Process(double delta)
		{
			
		}

		private void UpdateTheme(Theme NewTheme)
		{
			RenderingServer.SetDefaultClearColor(NewTheme.GetColor(Theme.Palette.BACKGROUND));
		}


		public void SetUserDataInstance(UserData Instance)
		{
			if (Instance == null) return;
			GetNode<TextureProgressBar>("LoadingBar").Value += 15f;
			UserDataInstance = Instance;
			UpdateTheme(Instance.GetTheme());
		}
	}
}