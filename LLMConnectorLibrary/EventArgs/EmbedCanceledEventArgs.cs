using LLMConnectorLibrary.Models;
using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class EmbedCanceledEventArgs : EmbedProgressEventArgs
	{
		public bool Cancel { get; }
		public bool Error { get; }
		public Exception? Exception { get; }

		internal EmbedCanceledEventArgs(IModel model, IEnumerable<(int key, string description)> values, object? tag) :
			this(model: model, values: values, tag: tag, cancel: true, error: false, exception: null)
		{ }

		internal EmbedCanceledEventArgs(IModel model, IEnumerable<(int key, string description)> values, object? tag, bool cancel, bool error, Exception? exception) :
			base(model: model, values: values, tag: tag)
		{
			Cancel = cancel;
			Error = error;
			Exception = exception;
		}
	}
}
