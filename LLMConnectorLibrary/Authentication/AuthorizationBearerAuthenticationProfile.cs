using GigaChat;
using OpenAI;
using System;
using System.Net.Http;

namespace LLMConnectorLibrary.Authentication
{
	public class AuthorizationBearerAuthenticationProfile<T>
		: AuthenticationProfile, IAuthorizationBearerAuthenticationProfile<T>
		where T : class
	{
		protected AuthorizationBearerAuthenticationProfile(IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
			: base(clientOptions: clientOptions)
		{
			ApiType = GetApiType();
			LLMBaseClient = CreateBaseClient(options: clientOptions, key: accessToken);
			AccessToken = accessToken;
			AccessTokenLifeTime = accessTokenLifeTime;
		}

		private AuthorizationBearerAuthenticationProfile(string caption, IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
			: this(
				  clientOptions: clientOptions,
				  accessToken: accessToken,
				  accessTokenLifeTime: accessTokenLifeTime
				  )
		{
			Caption = caption;
		}

		public override AuthenticationTypeEnum AuthenticationType { get; } = AuthenticationTypeEnum.AuthorizationBearer;

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
		protected static T CreateBaseClient(IClientOptions options, string key)
		{
			Type type = typeof(T);
			if (type.Name.Equals(nameof(OpenAIClient)))
			{
				return OpenAIService.CreateOpenAIClient(options: options, key: key) as T;
			}
			else if (type.Name.Equals(nameof(GigaChatClient)))
			{
				return GigaChatService.CreateGigaChatClient(options: options, key: key) as T;
			}
			else
			{
				throw new ArgumentException();
			}
		}
		
		protected static ApiTypeEnum GetApiType()
		{
			Type type = typeof(T);
			if (type.Name.Equals(nameof(OpenAIClient)))
			{
				return ApiTypeEnum.OpenAI;
			}
			else if (type.Name.Equals(nameof(GigaChatClient)))
			{
				return ApiTypeEnum.GigaChat;
			}
			else
			{
				throw new ArgumentException();
			}
		}
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.

		public override object LLMBaseClient { get; }

		public override HttpClient OauthHttpClient => HttpClient;

		T IAuthorizationBearerAuthenticationProfile<T>.LLMBaseClient => (T)LLMBaseClient;

		public override ApiTypeEnum ApiType { get; }

		public string AccessToken { get; }

		public TimeSpan AccessTokenLifeTime { get; }

		public override IAuthenticationProfile Clone(IClientOptions clientOptions)
			=> new AuthorizationBearerAuthenticationProfile<T>(caption: Caption, clientOptions: clientOptions, accessToken: AccessToken, accessTokenLifeTime: AccessTokenLifeTime);
		public override IAuthenticationProfile Clone(int pingTimeout, TimeSpan timeout, Uri endpoint)
			=> new AuthorizationBearerAuthenticationProfile<T>(caption: Caption, clientOptions: new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint), accessToken: AccessToken, accessTokenLifeTime: AccessTokenLifeTime);

		public static IAuthenticationProfile Create(string caption, int pingTimeout, TimeSpan timeout, Uri endpoint, string accessToken, TimeSpan accessTokenLifeTime)
			=> new AuthorizationBearerAuthenticationProfile<T>(caption: caption, new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint), accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime);

		public static IAuthenticationProfile Create(IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
			=> new AuthorizationBearerAuthenticationProfile<T>(clientOptions: clientOptions, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime);
	}
}
