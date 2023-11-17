using System;
using Godot;

#nullable enable
namespace OpenVoice
{
	public partial class Main : Node2D
	{
		private RequestHandler? RequestHandle;
		public override void _Ready()
		{
			LoadServerSidebar();
			RequestHandle = new RequestHandler();
		}

		private void LoadServerSidebar()
		{
			string[] Servers = FileAccess.Open("user://servers.dat", FileAccess.ModeFlags.Read).GetAsText().Split("\n");
			foreach (string Server in Servers)
			{
				string[] Parameters = Server.Split("/"); // Format: ip=0.0.0.0/logo="absolute_path"

				Control NewServerListInstance = (Control) GD.Load<PackedScene>("res://scenes/interactables/ServerListItem.tscn").Instantiate();

				NewServerListInstance.GetNode<Sprite2D>("Logo").Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(Parameters[1].Replace("logo=","").Replace("\"","")));
				NewServerListInstance.GetNode<Sprite2D>("Logo").Scale = new Vector2(50f / NewServerListInstance.GetNode<Sprite2D>("Logo").Texture.GetWidth(), 50f / NewServerListInstance.GetNode<Sprite2D>("Logo").Texture.GetHeight());

				Action OpenServerPressed = () => { LoadServer(Parameters[0].Replace("ip=", "")); };
				NewServerListInstance.GetNode<Button>("OpenServer").Connect("pressed", Callable.From(OpenServerPressed));
				
				GetNode<VBoxContainer>("ServerList/VBox").AddChild(NewServerListInstance);
			}

			// Repositioning AddServer Button to be at the bottom
			GetNode<VBoxContainer>("ServerList/VBox").MoveChild(GetNode<Control>("ServerList/VBox/AddServerItem"), GetNode<VBoxContainer>("ServerList/VBox").GetChildCount() - 1);
		}

		private void LoadServer(string IpAdress, bool tryReconnectIfFailed = true)
		{
			if (RequestHandle == null) return;
			RequestHandler.RequestError Error = RequestHandle.SubscribeToServer(IpAdress);
			if (Error == RequestHandler.RequestError.Ok) { GD.Print("Successfully connected to: " + IpAdress); }
			else if (Error == RequestHandler.RequestError.AlreadySubscribed) { RequestHandle.Unsubscribe(); if (tryReconnectIfFailed) LoadServer(IpAdress, false); return; }
			
			foreach (Channel ServerChannel in RequestHandle.GetSubscribed().GetChannels())
			{
				// ! Load Channel scene into "ChannelList/VBox"
			}
		}

		private void LoadChennel(int ChannelID)
		{
			if (RequestHandle.GetSubscribed().GetChannel(ChannelID) == null) return;
			if (RequestHandle.RequestChannelData(GetSubscribed().GetChannel(ChannelID)) != null);
		}

		public void UpdateTheme(Theme NewTheme)
		{
			foreach (Button Secondaries in GetNode<Node2D>("SecondaryBackgrounds").GetChildren())
			{
				Secondaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.SECONDARY));
			}
			foreach (Button Tertiaries in GetNode<Node2D>("TertiaryBackgrounds").GetChildren())
			{
				Tertiaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.BACKGROUND));
			}
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY));
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeStyleboxOverride("hover", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT));
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeStyleboxOverride("pressed", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY));
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
		}
	}
}