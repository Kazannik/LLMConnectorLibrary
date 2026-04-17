// Ignore Spelling: Uri

using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.EventArgs
{
	public class ModelsCollectionCompletedEventArgs : System.EventArgs
	{
		public Uri Uri { get; }
		public IEnumerable<string> Models { get; }
		public object? Tag { get; }

		internal ModelsCollectionCompletedEventArgs(Uri uri, IEnumerable<string> models, object? tag) : base()
		{
			Uri = uri;
			Models = models;
			Tag = tag;
		}
	}
}
