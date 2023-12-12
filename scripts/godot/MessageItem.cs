using Godot;
using System;

public partial class MessageItem : Control
{
	public override void _Process(double delta)
	{
		CustomMinimumSize = new Vector2(1278, 25 + GetNode<RichTextLabel>("MessageContent").Size.Y);
		GetNode<Sprite2D>("Avatar").Scale = Vector2.One * (40f / GetNode<Sprite2D>("Avatar").Texture.GetWidth());
	}
}
