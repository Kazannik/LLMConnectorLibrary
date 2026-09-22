using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LLMConnectorLibrary.Authentication
{
	public class PreAuthenticationProfile<T>: AuthorizationBearerAuthenticationProfile<T>, IPreAuthenticationProfile<T>
		where T : class
	{

		private T llmBaseClient;

		private PreAuthenticationProfile(IClientOptions oauthClientOptions, TimeSpan oauthAccessTokenLifeTime, IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
			: base (clientOptions: clientOptions, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime)
		{
			llmBaseClient = CreateBaseClient(options: clientOptions, key: "MSWORD_API_KEY");
			accessTokenBegin = DateTime.MinValue;
			OauthBaseClient = CreateBaseClient(options: oauthClientOptions, key: accessToken);
			OauthClientOptions = oauthClientOptions;
			OauthHttpClient = CreateClient(options: OauthClientOptions);
			OauthAccessTokenLifeTime = oauthAccessTokenLifeTime;
		}

		private PreAuthenticationProfile(string caption, IClientOptions oauthClientOptions, TimeSpan oauthAccessTokenLifeTime, IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
			: this(oauthClientOptions: oauthClientOptions, oauthAccessTokenLifeTime: oauthAccessTokenLifeTime, clientOptions: clientOptions, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime)
		{
			Caption = caption;
		}

		public override object LLMBaseClient => llmBaseClient;

		private DateTime accessTokenBegin;

		public void SetAccessToken(string accessToken)
		{
			accessTokenBegin = DateTime.Now;
			llmBaseClient = CreateBaseClient(options: ClientOptions, key: accessToken);
		}

		public bool AccessTokenIsValid => accessTokenBegin.Add(AccessTokenLifeTime) > DateTime.Now.AddSeconds(-10);

		public override HttpClient OauthHttpClient { get; }

		T IPreAuthenticationProfile<T>.LLMBaseClient => (T)LLMBaseClient;

		public new AuthenticationTypeEnum AuthenticationType { get; } = AuthenticationTypeEnum.PreAuthentication;

		public IClientOptions OauthClientOptions { get; }

		public T OauthBaseClient { get; }

		public TimeSpan OauthAccessTokenLifeTime { get; }

		public bool OauthPing() => OauthPingAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		public async Task<bool> OauthPingAsync() => await PingAsync(host: OauthClientOptions.BaseAddress.AbsoluteUri, timeout: OauthClientOptions.PingTimeout).ConfigureAwait(false);

		public int CheckOauthHost() => CheckOauthHostAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		public async Task<int> CheckOauthHostAsync() => await CheckHostAsync(client: OauthHttpClient, OauthClientOptions.BaseAddress).ConfigureAwait(false);

		public IAuthenticationProfile Clone(IClientOptions oauthClientOptions, IClientOptions clientOptions)
			=> new PreAuthenticationProfile<T>(caption: Caption, oauthClientOptions: oauthClientOptions, oauthAccessTokenLifeTime: OauthAccessTokenLifeTime, clientOptions: clientOptions, accessToken: AccessToken, accessTokenLifeTime: AccessTokenLifeTime);
		public IAuthenticationProfile Clone(int oauthPingTimeout, TimeSpan oauthTimeout, Uri oauthEndpoint, int pingTimeout, TimeSpan timeout, Uri endpoint)
			=> new PreAuthenticationProfile<T>(caption: Caption, oauthClientOptions: new ClientOptions(pingTimeout: oauthPingTimeout, timeout: oauthTimeout, endpoint: oauthEndpoint), oauthAccessTokenLifeTime: OauthAccessTokenLifeTime, clientOptions: new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint), accessToken: AccessToken, accessTokenLifeTime: AccessTokenLifeTime);
		
		public static IAuthenticationProfile Create(string caption, int oauthPingTimeout, TimeSpan oauthTimeout, Uri oauthEndpoint, TimeSpan oauthAccessTokenLifeTime, int pingTimeout, TimeSpan timeout, Uri endpoint, string accessToken, TimeSpan accessTokenLifeTime)
			=> new PreAuthenticationProfile<T>(caption: caption, oauthClientOptions: new ClientOptions(pingTimeout: oauthPingTimeout, timeout: oauthTimeout, endpoint: oauthEndpoint), oauthAccessTokenLifeTime: oauthAccessTokenLifeTime , clientOptions: new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint), accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime);

		public static IAuthenticationProfile Create(IClientOptions oauthClientOptions, TimeSpan oauthAccessTokenLifeTime, IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
			=> new PreAuthenticationProfile<T>(oauthClientOptions: oauthClientOptions, oauthAccessTokenLifeTime: oauthAccessTokenLifeTime,  clientOptions: clientOptions, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime);
	}
}
