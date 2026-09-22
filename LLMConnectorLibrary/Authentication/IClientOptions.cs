using System;

namespace LLMConnectorLibrary.Authentication
{
	public interface IClientOptions
	{
		/// <summary>
		/// Milliseconds
		/// </summary>
		int PingTimeout { get; }
		TimeSpan Timeout { get; }
		Uri BaseAddress { get; }
		Uri Endpoint { get; }
	}
}
