using System.Collections.Generic;
using Godot;
using OpenVoice;

public partial class MessagesController : ScrollContainer
{
	List<Message> Messages;
	public override void _Ready()
	{
		Messages = new List<Message>();
	}

	public override void _Process(double delta)
	{

	}

	public void Clear()
	{
		foreach (Control Message in GetNode<VBoxContainer>("VBox").GetChildren())
		{ Message.QueueFree(); }
	}

	public void PushMessage(Message Msg)
	{
		Control NewMessageItem = GD.Load<PackedScene>("res://scenes/interactables/Message.tscn").Instantiate<Control>();
		NewMessageItem.GetNode<Label>("Username").Text = RequestHandler.GetSubscribed().GetUserByID(Msg.GetAuthor()).GetUsername();
		NewMessageItem.GetNode<Label>("Time").Text = Formats.Date.GetDateFromTimeStamp(Msg.GetTimeStamp()) + " - " + Formats.Time.GetTimeFromTimeStamp(Msg.GetTimeStamp());
		NewMessageItem.GetNode<RichTextLabel>("MessageContent").Text = Msg.GetContent();
		GetNode<VBoxContainer>("VBox").AddChild(NewMessageItem);
	}
}
