using LLMConnectorLibrary.Models;
using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class ChatCanceledEventArgs : ChatProgressEventArgs
	{
		public bool Cancel { get; }
		public bool Error { get; }
		public Exception? Exception { get; }

		internal ChatCanceledEventArgs(IModel model, IChatOptions options, string systemMessage, IEnumerable<string> userMessages, object? tag) :
			this(model: model, options: options, systemMessage: systemMessage, userMessages: userMessages, tag: tag, cancel: true, error: false, exception: null)
		{ }

		internal ChatCanceledEventArgs(IModel model, IChatOptions options, string systemMessage, IEnumerable<string> userMessages, object? tag, bool cancel, bool error, Exception? exception) :
			base(model: model, options: options, systemMessage: systemMessage, userMessages: userMessages, tag: tag)
		{
			Cancel = cancel;
			Error = error;
			Exception = exception;
		}
	}
}
