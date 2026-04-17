using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class EmbedProgressEventArgs : System.EventArgs
	{
		public string Model { get; }
		public IEnumerable<(int key, string description)> Values { get; }
		public object? Tag { get; }

		internal EmbedProgressEventArgs(string model, IEnumerable<(int key, string description)> values, object? tag) : base()
		{
			Model = model;
			Values = values;
			Tag = tag;
		}
	}
}
