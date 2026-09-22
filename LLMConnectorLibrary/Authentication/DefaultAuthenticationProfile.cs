using GigaChat;
using OpenAI;
using System;
using System.Net.Http;

namespace LLMConnectorLibrary.Authentication
{
	public class DefaultAuthenticationProfile<T> : AuthenticationProfile, IDefaultAuthenticationProfile<T>
		where T : class
	{

		private DefaultAuthenticationProfile(IClientOptions clientOptions)
			: base(clientOptions: clientOptions)
		{
			ApiType = GetApiType();
			LLMBaseClient = CreateBaseClient(options: clientOptions);
		}

		private DefaultAuthenticationProfile(string caption, IClientOptions clientOptions)
			: this(clientOptions: clientOptions)
		{
			Caption = caption;
		}

		public override AuthenticationTypeEnum AuthenticationType { get; } = AuthenticationTypeEnum.Default;

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
		protected static T CreateBaseClient(IClientOptions options)
		{
			Type type = typeof(T);
			if (type.Name.Equals(nameof(OpenAIClient)))
			{
				return OpenAIService.CreateOpenAIClient(options: options) as T;
			}
			else if (type.Name.Equals(nameof(GigaChatClient)))
			{
				return GigaChatService.CreateGigaChatClient(options: options) as T;
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
		 
		T IDefaultAuthenticationProfile<T>.LLMBaseClient => (T)LLMBaseClient;

		public override ApiTypeEnum ApiType { get; }

		public override IAuthenticationProfile Clone(IClientOptions clientOptions)
			=> new DefaultAuthenticationProfile<T>(caption: Caption, clientOptions: clientOptions);
		public override IAuthenticationProfile Clone(int pingTimeout, TimeSpan timeout, Uri endpoint)
			=> new DefaultAuthenticationProfile<T>(caption: Caption, clientOptions: new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint));

		public static IAuthenticationProfile Create(string caption, int pingTimeout, TimeSpan timeout, Uri endpoint)
			=> new DefaultAuthenticationProfile<T>(caption: caption, clientOptions: new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint));

		public static IAuthenticationProfile Create(IClientOptions clientOptions)
		=> new DefaultAuthenticationProfile<T>(clientOptions: clientOptions);
	}
}
