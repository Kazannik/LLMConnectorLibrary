using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class EmbedCompletedEventArgs : EmbedProgressEventArgs
	{
		public IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> Embedding { get; }

		internal EmbedCompletedEventArgs(string model, IEnumerable<(int key, string description)> values, object? tag, IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> embedding) :
			base(model: model, values: values, tag: tag)
		{
			Embedding = embedding;
		}
	}
}
