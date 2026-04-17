// Ignore Spelling: Utils uri

using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LLMConnectorLibrary.Utils
{
	public static class Net
	{
		public const int DEFAULT_TIMEOUT = 100;

		private static HttpClient? _httpClient;
		private static HttpClient GetHttpClient(int timeout = DEFAULT_TIMEOUT)
		{
			TimeSpan timeoutSpan = TimeSpan.FromSeconds(timeout);

			if (_httpClient == null || _httpClient.Timeout != timeoutSpan)
			{
				_httpClient = new HttpClient
				{
					Timeout = timeoutSpan
				};
			}
			return _httpClient;
		}

		public static bool CheckHostByPing(string hostNameOrAddress, int timeout = DEFAULT_TIMEOUT)
		{
			return CheckHostByPingAsync(hostNameOrAddress: hostNameOrAddress, timeout: timeout)
				.GetAwaiter().GetResult();
		}

		public static async Task<bool> CheckHostByPingAsync(string hostNameOrAddress, int timeout = DEFAULT_TIMEOUT)
		{
			Ping ping = new();
			try
			{
				PingReply reply = await ping.SendPingAsync(hostNameOrAddress, timeout);
				return reply.Status == IPStatus.Success;
			}
			catch (PingException)
			{
				return false;
			}
		}

		public static async Task<bool> CheckHostByHttpAsync(Uri uri, int timeout = DEFAULT_TIMEOUT)
		{
			return await Task.FromResult(CheckHostByHttp(uri: uri, timeout: timeout));
		}

		public static bool CheckHostByHttp(string host, int timeout = DEFAULT_TIMEOUT)
		{
			return CheckHostByHttp(new Uri(host), timeout);
		}

		public static bool CheckHostByHttp(Uri uri, int timeout = DEFAULT_TIMEOUT)
		{
			try
			{
			HttpClient client = GetHttpClient(timeout);
				HttpResponseMessage response = client.GetAsync(uri).GetAwaiter().GetResult();
				return response.StatusCode == System.Net.HttpStatusCode.OK;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
