// Ignore Spelling: Uri

using LLMConnectorLibrary.Authentication;
using LLMConnectorLibrary.Models;

namespace LLMConnectorLibrary.EventArgs
{
	public class ModelsCollectionCompletedEventArgs : System.EventArgs
	{
		public IAuthenticationProfile Profile { get; }
		public ModelsCollection Models { get; }
		public object? Tag { get; }

		internal ModelsCollectionCompletedEventArgs(IAuthenticationProfile profile, ModelsCollection models, object? tag) : base()
		{
			Profile = profile;
			Models = models;
			Tag = tag;
		}
	}
}
