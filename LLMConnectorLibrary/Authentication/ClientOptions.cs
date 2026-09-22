using System;

namespace LLMConnectorLibrary.Authentication
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="pingTimeout">Milliseconds</param>
	/// <param name="timeout"></param>
	/// <param name="endpoint"></param>
	internal class ClientOptions(int pingTimeout, TimeSpan timeout, Uri endpoint) : IClientOptions
	{
		public static readonly TimeSpan DEFAULT_TIMEOUT = new(0, 3, 0);

		public int PingTimeout => pingTimeout;
		public TimeSpan Timeout => (timeout != null | timeout != TimeSpan.Zero) ? timeout : DEFAULT_TIMEOUT;
		public Uri Endpoint => endpoint;
		public Uri BaseAddress => GetBaseAddress(endpoint);

		private static Uri GetBaseAddress(Uri endpoint)
		{
			string absoluteUri = endpoint.AbsoluteUri;
			string absolutePath = endpoint.AbsolutePath;
			return new Uri(absoluteUri[..^absolutePath.Length]);
		}
	};
}
