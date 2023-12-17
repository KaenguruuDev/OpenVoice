using System;
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
			ActiveUser = new User("Kaenguruu");
			LoadServerSidebar();
		}

		public override void _Process(double delta)
        {
			// Message Box Resize
			int MessageBoxOffset = (int) GetNode<TextEdit>("UserInput").Size.Y - 70;
			GetNode<TextEdit>("UserInput").Position = new Vector2(GetNode<TextEdit>("UserInput").Position.X, 910 - MessageBoxOffset);
			GetNode<ScrollContainer>("MessagesController").Size = new Vector2(GetNode<ScrollContainer>("MessagesController").Size.X, 875 - MessageBoxOffset);
			GetNode<TextEdit>("UserInput").Size = new Vector2(GetNode<TextEdit>("UserInput").Size.X, 70);
        }

		public override async void _Input(InputEvent @event)
		{
			if (@event is InputEventKey && ((InputEventKey) @event).AsTextPhysicalKeycode() == "Shift+Enter" && @event.IsPressed())
			{
				if (GetNode<TextEdit>("UserInput").HasFocus() && ActiveServer != null)
				{
					if (ActiveServer == null || ActiveChannel == null || ActiveUser == null) return;
					var NewMessage = new Message(ActiveUser.GetUUID(), GetNode<TextEdit>("UserInput").Text, DateTime.Now.Ticks);
					var Result = await ActiveServer.SendMessage(ActiveChannel, NewMessage);
					GD.Print(Result);
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
			if (! result) return; // return if load failed

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
			foreach (Button Secondaries in GetNode<Node2D>("SecondaryBackgrounds").GetChildren())
			{
				Secondaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.SECONDARY, new Vector4I(5, 5, 5, 5)));
			}
			foreach (Button Tertiaries in GetNode<Node2D>("TertiaryBackgrounds").GetChildren())
			{
				Tertiaries.AddThemeStyleboxOverride("disabled", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.BACKGROUND, new Vector4I(5, 5, 5, 5)));
			}
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeStyleboxOverride("hover", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.ACCENT, new Vector4I(8, 8, 8, 8)));
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeStyleboxOverride("pressed", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.BUTTON, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
			GetNode<Button>("ServerList/VBox/AddServerItem/AddServer").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));

			GetNode<TextEdit>("UserInput").AddThemeStyleboxOverride("normal", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.BACKGROUND, new Vector4I(8, 8, 8, 8)));
			GetNode<TextEdit>("UserInput").AddThemeStyleboxOverride("focus", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.PRIMARY, new Vector4I(8, 8, 8, 8)));
			GetNode<TextEdit>("UserInput").AddThemeStyleboxOverride("read_only", NewTheme.GenerateStyleBoxFromTheme(Theme.StyleBoxType.FLAT, Theme.StyleTarget.LINE_EDIT, Theme.Palette.BACKGROUND, new Vector4I(8, 8, 8, 8)));
			GetNode<TextEdit>("UserInput").AddThemeColorOverride("font_color", NewTheme.GetColor(Theme.Palette.TEXT));

			LastTheme = NewTheme;
		}
	}
}