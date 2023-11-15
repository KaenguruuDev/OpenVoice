using Godot;

namespace OpenVoice
{
	public partial class Launch : Node2D
	{
		private UserData UserDataInstance;


		private async void LoadData()
		{
			while (UserDataInstance == null) { }
			GetTree().Root.CallDeferred("add_child", GD.Load<PackedScene>("Home.tscn").Instantiate());
			QueueFree();
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			LoadData();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			
		}

		public void SetUserDataInstance(UserData Instance)
		{
			if (Instance == null) return;
			GetNode<TextureProgressBar>("LoadingBar").Value += 15f;
			UserDataInstance = Instance;
		}
	}
}