// Ignore Spelling: Uri

using LLMConnectorLibrary.Authentication;

namespace LLMConnectorLibrary.EventArgs
{
	public class CheckHostEventArgs : System.EventArgs
	{
		public IAuthenticationProfile Profile { get; }
		public bool IsAvailable { get; }
		public object? Tag { get; }

		internal CheckHostEventArgs(IAuthenticationProfile profile, bool isAvailable, object? tag) : base()
		{
			Profile = profile;
			IsAvailable = isAvailable;
			Tag = tag;
		}
	}
}
