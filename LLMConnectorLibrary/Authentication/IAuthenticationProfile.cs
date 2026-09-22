using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LLMConnectorLibrary.Authentication
{
	public interface IAuthenticationProfile
	{
		string Caption { get; set; }
		ApiTypeEnum ApiType { get; }
		AuthenticationTypeEnum AuthenticationType { get; }
		IClientOptions ClientOptions { get; }
		bool Ping();
		Task<bool> PingAsync();
		int CheckHost();
		Task<int> CheckHostAsync();
		HttpClient HttpClient { get; }
		HttpClient OauthHttpClient { get; }
		object LLMBaseClient { get; }

		IAuthenticationProfile Clone(IClientOptions clientOptions);

		IAuthenticationProfile Clone(int pingTimeout, TimeSpan timeout, Uri endpoint);
	}
}
