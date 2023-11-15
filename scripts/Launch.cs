using Godot;
using System;

public partial class Launch : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DisplayServer.WindowSetSize(new Vector2I(265, 310));
		DisplayServer.Center();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(DisplayServer.WindowGetSize());
	}
}
