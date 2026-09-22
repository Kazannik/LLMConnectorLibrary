using LLMConnectorLibrary.Models;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class EmbedProgressEventArgs : System.EventArgs
	{
		public IModel Model { get; }
		public IEnumerable<(int key, string description)> Values { get; }
		public object? Tag { get; }

		internal EmbedProgressEventArgs(IModel model, IEnumerable<(int key, string description)> values, object? tag) : base()
		{
			Model = model;
			Values = values;
			Tag = tag;
		}
	}
}
