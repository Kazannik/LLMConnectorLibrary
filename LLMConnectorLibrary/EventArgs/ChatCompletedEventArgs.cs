using LLMConnectorLibrary.Models;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class ChatCompletedEventArgs : ChatProgressEventArgs
	{
		public string Message { get; }

		internal ChatCompletedEventArgs(IModel model, IChatOptions options, string systemMessage, IEnumerable<string> userMessages, object? tag, string message) :
			base(model: model, options: options, systemMessage: systemMessage, userMessages: userMessages, tag: tag)
		{
			Message = message;
		}
	}
}
