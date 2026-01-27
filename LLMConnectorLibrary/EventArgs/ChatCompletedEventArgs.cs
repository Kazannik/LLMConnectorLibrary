using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class ChatCompletedEventArgs : ChatProgressEventArgs
	{
		public string Message { get; }

		internal ChatCompletedEventArgs(string model, string systemMessage, IEnumerable<string> userMessages, object tag, string message) :
			base(model: model, systemMessage: systemMessage, userMessages: userMessages, tag: tag)
		{
			Message = message;
		}
	}
}
