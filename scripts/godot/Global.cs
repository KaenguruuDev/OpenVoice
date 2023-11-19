using Godot;

namespace OpenVoice
{
	public partial class Global : Node
	{
		private User ActiveUser;

		public User GetUser()
		{ return ActiveUser; }
		public void SetUser(User ActiveUser)
		{ this.ActiveUser = ActiveUser; }
	}
}