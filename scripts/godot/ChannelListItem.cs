using Godot;

namespace OpenVoice
{
	public partial class ChannelListItem : Control
	{
		public override void _Ready()
		{

		}
		public override void _Process(double delta)
		{

		}

		public void UpdateTheme(Theme NewTheme)
		{
			((ShaderMaterial) GetNode<Sprite2D>("Icon").Material).SetShaderParameter("PrimaryValue", NewTheme.GetColor(OpenVoice.Theme.Palette.PRIMARY));
			((ShaderMaterial) GetNode<Sprite2D>("Icon").Material).SetShaderParameter("AccentValue", NewTheme.GetColor(OpenVoice.Theme.Palette.ACCENT));
		}
	}
}