// Ignore Spelling: Utils

using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LLMConnectorLibrary.Utils
{
	public static class Net
	{
		public static bool CheckHostByPing(string hostNameOrAddress, int timeout = 1000)
		{
			Task<bool> task = CheckHostByPingAsync(hostNameOrAddress: hostNameOrAddress, timeout: timeout);
			task.Wait();
			return task.Result;
		}

		public static async Task<bool> CheckHostByPingAsync(string hostNameOrAddress, int timeout = 1000)
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

		public static bool CheckHostByHttp(Uri uri, double timeout = 10)
		{
			Task<bool> task = CheckHostByHttpAsync(uri: uri, timeout: timeout);
			task.Wait();
			return task.Result;
		}

		public static async Task<bool> CheckHostByHttpAsync(Uri uri, double timeout = 10)
		{
			HttpClient client = new()
			{
				Timeout = TimeSpan.FromSeconds(timeout)
			};
			try
			{
				HttpResponseMessage response = client.GetAsync(uri).GetAwaiter().GetResult();
				return response.StatusCode == System.Net.HttpStatusCode.OK;
			}
			catch (HttpRequestException)
			{
				return false;
			}
			catch (TaskCanceledException)
			{
				return false;
			}
		}
	}
}
