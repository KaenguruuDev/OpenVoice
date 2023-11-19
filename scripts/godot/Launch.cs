using System.IO;
using Godot;

namespace OpenVoice
{
	public partial class Launch : Node2D
	{
		// ! Implement ensure_files_exist

		private async void LoadData()
		{
			Directory.CreateDirectory(OS.GetUserDataDir() + "/users");
			UserData.LoadData();
			UpdateTheme(UserData.GetTheme());
			GetTree().Root.CallDeferred("add_child", GD.Load<PackedScene>("WindowController.tscn").Instantiate());
			QueueFree();
		}

		public override void _Ready()
		{
			DisplayServer.WindowSetSize(new Vector2I(260, 310));
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true);
			LoadData();
		}

		public override void _Process(double delta)
		{
			
		}

		private void UpdateTheme(Theme NewTheme)
		{
			RenderingServer.SetDefaultClearColor(NewTheme.GetColor(Theme.Palette.BACKGROUND));
		}
	}
}