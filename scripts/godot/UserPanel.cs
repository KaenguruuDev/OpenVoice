using Godot;
using System;

namespace OpenVoice
{
	public partial class UserPanel : Control
	{
		public void LoadPanel()
		{
			GetNode<Label>("Username").Text = GetNode<Global>("/root/Global").GetUser().GetUsername();
		}

		public override void _Process(double delta)
		{

		}
	}
}