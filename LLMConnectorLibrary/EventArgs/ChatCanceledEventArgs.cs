using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class ChatCanceledEventArgs : ChatProgressEventArgs
	{
		public bool Cancel { get; }
		public bool Error { get; }
		public Exception Exception { get; }

		internal ChatCanceledEventArgs(string model, string systemMessage, IEnumerable<string> userMessages, object tag) :
			this(model: model, systemMessage: systemMessage, userMessages: userMessages, tag: tag, cancel: true, error: false, exception: null)
		{ }

		internal ChatCanceledEventArgs(string model, string systemMessage, IEnumerable<string> userMessages, object tag, bool cancel, bool error, Exception exception) :
			base(model: model, systemMessage: systemMessage, userMessages: userMessages, tag: tag)
		{
			Cancel = cancel;
			Error = error;
			Exception = exception;
		}
	}
}
