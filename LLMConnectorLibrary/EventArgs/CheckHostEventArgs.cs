// Ignore Spelling: Uri

using System;

namespace LLMConnectorLibrary.EventArgs
{
	public class CheckHostEventArgs : System.EventArgs
	{
		public Uri Uri { get; }
		public TimeSpan Timeout { get; }
		public bool IsAvailable { get; }
		public object? Tag { get; }

		internal CheckHostEventArgs(Uri uri, TimeSpan timeout, bool isAvailable, object? tag) : base()
		{
			Uri = uri;
			Timeout = timeout;
			IsAvailable = isAvailable;
			Tag = tag;
		}
	}
}
