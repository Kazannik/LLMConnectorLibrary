using LLMConnectorLibrary.Models;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class ChatProgressEventArgs : System.EventArgs
	{
		public IModel Model { get; }

		public IChatOptions Options { get; }
		public string SystemMessage { get; }
		public IEnumerable<string> UserMessages { get; }
		public object? Tag { get; }

		internal ChatProgressEventArgs(IModel model, IChatOptions options, string systemMessage, IEnumerable<string> userMessages, object? tag) : base()
		{
			Model = model;
			Options = options;
			SystemMessage = systemMessage;
			UserMessages = userMessages;
			Tag = tag;
		}
	}
}
