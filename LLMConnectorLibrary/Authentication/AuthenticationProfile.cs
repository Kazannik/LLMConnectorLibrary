using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LLMConnectorLibrary.Authentication
{
	public abstract class AuthenticationProfile(IClientOptions clientOptions, HttpClient httpClient) : IAuthenticationProfile
	{
		protected AuthenticationProfile(IClientOptions clientOptions) 
			: this(
				  clientOptions:  clientOptions,
				  httpClient: CreateClient(options: clientOptions)
				  ) { }

		protected static HttpClient CreateClient(IClientOptions options)
		{
			HttpClient client = new();
			if (options != null && options.Endpoint != null)
				client.BaseAddress = options.Endpoint;

			if (options != null && options.Timeout != null && options.Timeout != TimeSpan.Zero)
				client.Timeout = options.Timeout;

			return client;
		}
				
		public string Caption { get; set; } = string.Empty;

		public abstract ApiTypeEnum ApiType { get; }

		public abstract AuthenticationTypeEnum AuthenticationType { get; }

		public IClientOptions ClientOptions { get; } = clientOptions;

		public HttpClient HttpClient { get; } = httpClient;

		public abstract HttpClient OauthHttpClient { get; }

		public abstract object LLMBaseClient { get; }

		public bool Ping() => PingAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		public async Task<bool> PingAsync() => await PingAsync(host: GetBaseAddress(ClientOptions.BaseAddress).Host, timeout: ClientOptions.PingTimeout).ConfigureAwait(false);

		protected async Task<bool> PingAsync(string host, int timeout)
		{
#if AI_MESSAGE_DEBUG
			return await Task<bool>.Factory.StartNew(() => {
				System.Threading.Thread.Sleep(500);
				return true;
			});
#else
			Ping ping = new();
			try
			{
				PingReply reply = await ping.SendPingAsync(hostNameOrAddress: host, timeout: timeout)
					.ConfigureAwait(false);
				return reply.Status == IPStatus.Success;
			}
			catch (Exception)
			{
				return false;
			}
#endif
		}

		public int CheckHost() => CheckHostAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		public async Task<int> CheckHostAsync() => await CheckHostAsync(HttpClient, ClientOptions.BaseAddress).ConfigureAwait(false);

		protected async Task<int> CheckHostAsync(HttpClient client, Uri requestUri)
		{
#if AI_MESSAGE_DEBUG
			return await Task<int>.Factory.StartNew(() => {
				System.Threading.Thread.Sleep(1000);				
				return 200;
			});
#else
			try
			{
				using HttpResponseMessage response = await client.GetAsync(requestUri: requestUri)
					.ConfigureAwait(false);
				return (int)response.StatusCode;
			}
			catch (Exception)
			{
				return 0;
			}
#endif
		}

		protected static Uri GetBaseAddress(Uri endpoint)
		{
			string absoluteUri = endpoint.AbsoluteUri;
			string absolutePath = endpoint.AbsolutePath;
			return new Uri(absoluteUri[..^absolutePath.Length]);
		}

		public abstract IAuthenticationProfile Clone(IClientOptions ClientOptions);
		public abstract IAuthenticationProfile Clone(int pingTimeout, TimeSpan timeout, Uri endpoint);
	}
}
