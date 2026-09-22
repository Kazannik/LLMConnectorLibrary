using GigaChat;
using OpenAI;
using System;
using System.Security.Cryptography.X509Certificates;

namespace LLMConnectorLibrary.Authentication
{
	public static class AuthenticationProfileFactory
	{
		public static IAuthenticationProfile DefaultAuthenticationProfile(ApiTypeEnum api, IClientOptions clientOptions)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => DefaultAuthenticationProfile<OpenAIClient>.Create(clientOptions),
				ApiTypeEnum.GigaChat => DefaultAuthenticationProfile<GigaChatClient>.Create(clientOptions),
				_ => throw new NotImplementedException(),
			};
		}

		public static IAuthenticationProfile DefaultAuthenticationProfile(ApiTypeEnum api, string caption, int pingTimeout, TimeSpan timeout, Uri endpoint)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => DefaultAuthenticationProfile<OpenAIClient>.Create(caption: caption, pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint),
				ApiTypeEnum.GigaChat => DefaultAuthenticationProfile<GigaChatClient>.Create(caption: caption, pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint),
				_ => throw new NotImplementedException(),
			};
		}

		public static IAuthenticationProfile CertificateAuthenticationProfile(ApiTypeEnum api, IClientOptions clientOptions, X509Certificate2 certificate)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => CertificateAuthenticationProfile<OpenAIClient>.Create(clientOptions, certificate),
				ApiTypeEnum.GigaChat => CertificateAuthenticationProfile<GigaChatClient>.Create(clientOptions, certificate),
				_ => throw new NotImplementedException(),
			};
		}

		public static IAuthenticationProfile CertificateAuthenticationProfile(ApiTypeEnum api, string caption, int pingTimeout, TimeSpan timeout, Uri endpoint, X509Certificate2 certificate)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => CertificateAuthenticationProfile<OpenAIClient>.Create(caption: caption, pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint, certificate),
				ApiTypeEnum.GigaChat => CertificateAuthenticationProfile<GigaChatClient>.Create(caption: caption, pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint, certificate),
				_ => throw new NotImplementedException(),
			};
		}

		public static IAuthenticationProfile AuthorizationBearerAuthenticationProfile(ApiTypeEnum api, IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => AuthorizationBearerAuthenticationProfile<OpenAIClient>.Create(clientOptions, accessToken, accessTokenLifeTime: accessTokenLifeTime),
				ApiTypeEnum.GigaChat => AuthorizationBearerAuthenticationProfile<GigaChatClient>.Create(clientOptions, accessToken, accessTokenLifeTime: accessTokenLifeTime),
				_ => throw new NotImplementedException(),
			};
		}

		public static IAuthenticationProfile AuthorizationBearerAuthenticationProfile(ApiTypeEnum api, string caption, int pingTimeout, TimeSpan timeout, Uri endpoint, string accessToken, TimeSpan accessTokenLifeTime)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => AuthorizationBearerAuthenticationProfile<OpenAIClient>.Create(caption: caption, pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime),
				ApiTypeEnum.GigaChat => AuthorizationBearerAuthenticationProfile<GigaChatClient>.Create(caption: caption, pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime),
				_ => throw new NotImplementedException(),
			};
		}

		public static IAuthenticationProfile PreAuthenticationProfile(ApiTypeEnum api, IClientOptions oauthClientOptions, TimeSpan oauthAccessTokenLifeTime, IClientOptions clientOptions, string accessToken, TimeSpan accessTokenLifeTime)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => PreAuthenticationProfile<OpenAIClient>.Create(oauthClientOptions: oauthClientOptions, oauthAccessTokenLifeTime: oauthAccessTokenLifeTime,  clientOptions, accessToken, accessTokenLifeTime: accessTokenLifeTime),
				ApiTypeEnum.GigaChat => PreAuthenticationProfile<GigaChatClient>.Create(oauthClientOptions: oauthClientOptions, oauthAccessTokenLifeTime: oauthAccessTokenLifeTime, clientOptions, accessToken, accessTokenLifeTime: accessTokenLifeTime),
				_ => throw new NotImplementedException(),
			};
		}

		public static IAuthenticationProfile PreAuthenticationProfile(ApiTypeEnum api, string caption, int oauthPingTimeout, TimeSpan oauthTimeout, Uri oauthEndpoint, TimeSpan oauthAccessTokenLifeTime, int pingTimeout, TimeSpan timeout, Uri endpoint, string accessToken, TimeSpan accessTokenLifeTime)
		{
			return api switch
			{
				ApiTypeEnum.OpenAI => PreAuthenticationProfile<OpenAIClient>.Create(caption: caption, oauthPingTimeout: oauthPingTimeout, oauthTimeout: oauthTimeout, oauthEndpoint: oauthEndpoint, oauthAccessTokenLifeTime: oauthAccessTokenLifeTime,  pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime),
				ApiTypeEnum.GigaChat => PreAuthenticationProfile<GigaChatClient>.Create(caption: caption, oauthPingTimeout: oauthPingTimeout, oauthTimeout: oauthTimeout, oauthEndpoint: oauthEndpoint, oauthAccessTokenLifeTime: oauthAccessTokenLifeTime, pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint, accessToken: accessToken, accessTokenLifeTime: accessTokenLifeTime),
				_ => throw new NotImplementedException(),
			};
		}

#if AI_MESSAGE_DEBUG
		public static readonly IAuthenticationProfile TestProfile =
			DefaultAuthenticationProfile<OpenAIClient>.Create(
				caption: "Test Profile",
				pingTimeout: 100,
				timeout: new TimeSpan(0, 3, 0),
				endpoint: new Uri("http://localhost"));
#endif

		public static readonly IAuthenticationProfile Default = 
			DefaultAuthenticationProfile<OpenAIClient>.Create(
			caption: "Default",
			pingTimeout: 100,
			timeout: new TimeSpan(0, 3, 0),
			endpoint: new Uri("http://localhost"));
		
		public static readonly IAuthenticationProfile OllamaLocal = 
			DefaultAuthenticationProfile<OpenAIClient>.Create(
			caption: "Ollama (local)",
			pingTimeout: 100,
			timeout: new TimeSpan(0, 3, 0),
			endpoint: new Uri("http://localhost:11434/v1"));
		
		public static readonly IAuthenticationProfile GigaChat = 
			PreAuthenticationProfile<GigaChatClient>.Create(
				caption: "GigaChat (СБЕР)",
				oauthPingTimeout: 0,
				oauthTimeout: new TimeSpan(0, 3, 0),
				oauthEndpoint: new Uri("https://ngw.devices.sberbank.ru:9443/api/v2/oauth"),
				oauthAccessTokenLifeTime: new TimeSpan(0, 25, 0),
				pingTimeout: 0,
				timeout: new TimeSpan(0, 3, 0),
				endpoint: new Uri("https://gigachat.devices.sberbank.ru/api/v1"),
				accessToken: "Токен, полученный в личном кабинете: https://developers.sber.ru/studio/workspaces/",
				accessTokenLifeTime: TimeSpan.Zero);
		
		public static readonly IAuthenticationProfile RTK = 
			DefaultAuthenticationProfile<OpenAIClient>.Create(
				caption: "РТК",
				pingTimeout: 100,
				timeout: new TimeSpan(0, 3, 0),
				endpoint: new Uri("http://10.197.24.37:8000/v1"));
		
		public static readonly IAuthenticationProfile Sber = 
			CertificateAuthenticationProfile<GigaChatClient>.Create(
				caption: "СБЕР (ЦРТ)",
				pingTimeout: 0,
				timeout: new TimeSpan(0, 3, 0),
				endpoint: new Uri("https://10.32.1.122:8080/v1"),
				certificate: null);
	}
}
