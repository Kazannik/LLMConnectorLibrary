using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class EmbedProgressEventArgs : System.EventArgs
	{
		public string Model { get; }
		public IEnumerable<string> Values { get; }
		public object Tag { get; }

		internal EmbedProgressEventArgs(string model, IEnumerable<string> values, object tag) : base()
		{
			Model = model;
			Values = values;
			Tag = tag;
		}
	}
}
