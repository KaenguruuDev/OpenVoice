using Godot;
using System;

public partial class Home : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProjectSettings.SetSetting("window/size/resizable", true);

		DisplayServer.WindowSetSize(new Vector2I(1152, 648));
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, false);
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(GetChildCount());
	}
}
