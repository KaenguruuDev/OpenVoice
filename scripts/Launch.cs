using Godot;

/*
 Your typical launch window loading updates and similar.
*/

public partial class Launch : Node2D
{
	public override void _Ready()
	{
		DisplayServer.WindowSetSize(new Vector2I(500, 500));
		ProjectSettings.Borderless(true);
	}

	public override void _Process(double delta)
	{

	}
}
