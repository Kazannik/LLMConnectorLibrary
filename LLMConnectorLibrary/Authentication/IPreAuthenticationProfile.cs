using System;
using System.Threading.Tasks;

namespace LLMConnectorLibrary.Authentication
{
	public interface IPreAuthenticationProfile<T> : IAuthorizationBearerAuthenticationProfile<T>
		where T : class
	{
		new T LLMBaseClient { get; }
		void SetAccessToken(string accessToken);
		bool AccessTokenIsValid { get; }
		IClientOptions OauthClientOptions { get; }
		T OauthBaseClient { get; }
		TimeSpan OauthAccessTokenLifeTime { get; }
		bool OauthPing();
		Task<bool> OauthPingAsync();
		int CheckOauthHost();
		Task<int> CheckOauthHostAsync();
		IAuthenticationProfile Clone(IClientOptions oauthClientOptions, IClientOptions clientOptions);
		IAuthenticationProfile Clone(int oauthPingTimeout, TimeSpan oauthTimeout, Uri oauthEndpoint, int pingTimeout, TimeSpan timeout, Uri endpoint);
	}
}
