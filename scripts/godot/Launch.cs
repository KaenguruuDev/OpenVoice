using System.IO;
using Godot;

namespace OpenVoice
{
	public partial class Launch : Node2D
	{
		// ! Implement ensure_files_exist

		private void LoadData()
		{
			Directory.CreateDirectory(OS.GetUserDataDir() + "/users");
			Directory.CreateDirectory(OS.GetUserDataDir() + "/sessions");
			UserData.LoadData();
			UpdateTheme(UserData.GetTheme());
			GetTree().Root.CallDeferred("add_child", GD.Load<PackedScene>("res://scenes/WindowController.tscn").Instantiate());
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