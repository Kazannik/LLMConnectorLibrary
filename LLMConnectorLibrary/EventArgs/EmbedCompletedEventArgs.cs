using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class EmbedCompletedEventArgs : EmbedProgressEventArgs
	{
		public IEnumerable<ReadOnlyMemory<float>> Embedding { get; }

		internal EmbedCompletedEventArgs(string model, IEnumerable<string> values, object tag, IEnumerable<ReadOnlyMemory<float>> embedding) :
			base(model: model, values: values, tag: tag)
		{
			Embedding = embedding;
		}
	}
}
