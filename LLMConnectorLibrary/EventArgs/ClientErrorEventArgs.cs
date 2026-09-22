using System;

namespace LLMConnectorLibrary.EventArgs
{
	public class ClientErrorEventArgs : System.EventArgs
	{
		public Exception? Exception { get; }
		
		internal ClientErrorEventArgs(Exception? exception)
		{
			Exception = exception;
		}
	}
}
