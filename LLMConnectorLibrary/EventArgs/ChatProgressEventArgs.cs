using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class ChatProgressEventArgs : System.EventArgs
	{
		public string Model { get; }
		public string SystemMessage { get; }
		public IEnumerable<string> UserMessages { get; }
		public object Tag { get; }

		internal ChatProgressEventArgs(string model, string systemMessage, IEnumerable<string> userMessages, object tag) : base()
		{
			Model = model;
			SystemMessage = systemMessage;
			UserMessages = userMessages;
			Tag = tag;
		}
	}
}
