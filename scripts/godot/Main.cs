using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

#nullable enable
namespace OpenVoice
{
	public partial class Main : Node2D
	{
		private Theme? LastTheme;
		private User? ActiveUser;
		private Server? ActiveServer;
		private Channel? ActiveChannel;

		AudioEffectRecord? MicRecord;

		public override void _Ready()
		{
			// Connecting Signals
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").Pressed += () => { UIButtonAction(0); };
			GetNode<Button>("AddServerDialog/CenterContainer/HBoxContainer/Add").Pressed += () => { UIButtonAction(1); };
			GetNode<Button>("AddServerDialog/CenterContainer/HBoxContainer/Cancel").Pressed += () => { UIButtonAction(2); };


			ActiveUser = new User("Kaenguruu");
			LoadServerSidebar();
		}

		public override void _Process(double delta)
		{
			// Message Box Resize
			int MessageBoxOffset = (int)GetNode<TextEdit>("UserInput").Size.Y - 70;
			GetNode<TextEdit>("UserInput").Position = new Vector2(GetNode<TextEdit>("UserInput").Position.X, 910 - MessageBoxOffset);
			GetNode<ScrollContainer>("MessagesController").Size = new Vector2(GetNode<ScrollContainer>("MessagesController").Size.X, 875 - MessageBoxOffset);
			GetNode<TextEdit>("UserInput").Size = new Vector2(GetNode<TextEdit>("UserInput").Size.X, 70);
		}

		public override async void _Input(InputEvent @event)
		{
			if (@event is InputEventKey && ((InputEventKey)@event).AsTextPhysicalKeycode() == "Shift+Enter" && @event.IsPressed())
			{
				if (GetNode<TextEdit>("UserInput").HasFocus() && ActiveServer != null)
				{
					if (ActiveServer == null || ActiveChannel == null || ActiveUser == null) return;
					var NewMessage = new Message(ActiveUser.GetUUID(), GetNode<TextEdit>("UserInput").Text, DateTime.Now.Second);
					var Result = await ActiveServer.SendMessage(ActiveChannel, NewMessage);
					if (Result) { GetNode<MessagesController>("MessagesController").PushMessage(NewMessage); }
				}
			}
		}

		/*
		private void GetMicInput()
		{
			MicRecord = (AudioEffectRecord) AudioServer.GetBusEffect(AudioServer.GetBusIndex("Record"), 0);
			AudioEffectSpectrumAnalyzerInstance AnalyzerInstance = (AudioEffectSpectrumAnalyzerInstance) AudioServer.GetBusEffectInstance(AudioServer.GetBusIndex("Record"), 1);
			GetNode<TextureProgressBar>("UserPanel/MicStatus/MicLevel").Value = Math.Clamp((AnalyzerInstance.GetMagnitudeForFrequencyRange(0f, 50000f, AudioEffectSpectrumAnalyzerInstance.MagnitudeMode.Max) * 2000f).X, GetNode<TextureProgressBar>("UserPanel/MicStatus/MicLevel").Value - 3, GetNode<TextureProgressBar>("UserPanel/MicStatus/MicLevel").Value + 3);
		}
		*/
		/*
        private void LoadUserPanel()
		{
			if (ActiveUser == null) return;
		}
		*/

		private void UIButtonAction(int ID)
		{
			switch (ID)
			{
				// Add Server Button
				case 0:
					GetNode<Control>("AddServerDialog").Show();
					break;
				// AddServerDialog.Confirm
				case 1:
					List<string> Servers = FileAccess.Open("user://servers.dat", FileAccess.ModeFlags.Read).GetAsText().Split("\n").ToList();
					Servers.Add("ip=" + GetNode<LineEdit>("AddServerDialog/IPInput").Text + "/logo=\"\"");
					string SaveData = ""; foreach (string ServerData in Servers) { if (ServerData != "") SaveData += ServerData + "\n"; }
					{ using var file = FileAccess.Open("user://servers.dat", FileAccess.ModeFlags.Write); file.StoreString(SaveData); }

					GetNode<Control>("AddServerDialog").Hide();
					break;
				// AddServerDialog.Cancel
				case 2:
					GetNode<Control>("AddServerDialog").Hide();
					break;
				default:
					break;
			}
		}

		private void LoadServerSidebar()
		{
			using var file = FileAccess.Open("user://servers.dat", FileAccess.ModeFlags.Read);
			string[] Servers = file.GetAsText().Split("\n");

			if (Servers.Length == 1) return;

			foreach (string Server in Servers)
			{
				string[] Parameters = Server.Split("/"); // Format: ip=0.0.0.0/logo="absolute_path"

				Control NewServerListInstance = (Control)GD.Load<PackedScene>("res://scenes/interactables/ServerListItem.tscn").Instantiate();

				if (Parameters.Length < 2) return;

				if (FileAccess.FileExists(Parameters[1].Replace("logo=", "").Replace("\"", "")))
				{
					NewServerListInstance.GetNode<Sprite2D>("Logo").Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(Parameters[1].Replace("logo=", "").Replace("\"", "")));
					NewServerListInstance.GetNode<Sprite2D>("Logo").Scale = new Vector2(50f / NewServerListInstance.GetNode<Sprite2D>("Logo").Texture.GetWidth(), 50f / NewServerListInstance.GetNode<Sprite2D>("Logo").Texture.GetHeight());
				}

				Action OpenServerPressed = () => { LoadServer(Parameters[0].Replace("ip=", "")); };
				NewServerListInstance.GetNode<Button>("OpenServer").Connect("pressed", Callable.From(OpenServerPressed));

				GetNode<VBoxContainer>("ServerList/VBox").AddChild(NewServerListInstance);
			}

			// Repositioning AddServer Button to be at the bottom
			GetNode<VBoxContainer>("ServerList/VBox").MoveChild(GetNode<Control>("ServerList/VBox/AddServerItem"), GetNode<VBoxContainer>("ServerList/VBox").GetChildCount() - 1);
		}

		private async void LoadServer(string IpAdress, bool tryReconnectIfFailed = true)
		{
			if (ActiveUser == null || GetNode<HttpRequest>("HTTPRequest") == null) return;

			// Try to subscribe to the selected server
			Server RequestedServer = new Server("127.0.0.1", 9999, ActiveUser, GetNode<HttpRequest>("HTTPRequest"));
			RequestHandler.RequestError Error = await RequestHandler.SubscribeToServer(RequestedServer);

			// Early return if there's an error, consider showing error to user
			if (Error != RequestHandler.RequestError.Ok) return;
			ActiveServer = RequestedServer;

			// GD.Print("Successfully connected to: " + IpAdress);
			bool result = await RequestedServer.LoadData();
			if (!result) return; // return if load failed

			foreach (Channel ServerChannel in RequestedServer.GetChannels())
			{
				ChannelListItem NewChannelListItem = GD.Load<PackedScene>("res://scenes/interactables/ChannelListItem.tscn").Instantiate<ChannelListItem>();
				GetNode<VBoxContainer>("ChannelList/VBox").AddChild(NewChannelListItem);
				NewChannelListItem.GetNode<Button>("Open").Pressed += () => LoadChannel(ServerChannel.GetId());
				NewChannelListItem.UpdateTheme(LastTheme);
			}
		}

		private void LoadChannel(int ChannelID)
		{
			var Server = RequestHandler.GetSubscribed();
			if (Server == null || Server.GetChannel(ChannelID) == null) return;

			// Clear all messages
			GetNode<MessagesController>("MessagesController").Clear();

			var CH = RequestHandler.GetSubscribed()?.GetChannel(ChannelID);
			if (CH == null) return;
			ActiveChannel = CH;
			foreach (Message Msg in CH.GetMessages())
			{ GetNode<MessagesController>("MessagesController").PushMessage(Msg); }
		}

		public void UpdateTheme(Theme NewTheme)
		{
			foreach (Button Secondaries in GetTree().GetNodesInGroup("SecondaryBG"))
			{
				Secondaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.SECONDARY, new Vector4I(5, 5, 5, 5)));
			}
			foreach (Button Tertiaries in GetTree().GetNodesInGroup("TertiaryBG"))
			{
				Tertiaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.BACKGROUND, new Vector4I(5, 5, 5, 5)));
			}
			foreach (Button UIButtons in GetTree().GetNodesInGroup("UIButtons"))
			{
				UIButtons.AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
				UIButtons.AddThemeStyleboxOverride("hover", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT, new Vector4I(8, 8, 8, 8)));
				UIButtons.AddThemeStyleboxOverride("pressed", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
				UIButtons.AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
			}
			foreach (Control InputFields in GetTree().GetNodesInGroup("InputFields"))
			{
				InputFields.AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.BACKGROUND, new Vector4I(8, 8, 8, 8)));
				InputFields.AddThemeStyleboxOverride("focus", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
				InputFields.AddThemeStyleboxOverride("read_only", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.BACKGROUND, new Vector4I(8, 8, 8, 8)));
				InputFields.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.BACKGROUND, new Vector4I(8, 8, 8, 8)));
				InputFields.AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
			}
			foreach (Control TextFields in GetTree().GetNodesInGroup("Text"))
			{
				TextFields.AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));
			}

			LastTheme = NewTheme;
		}
	}
}