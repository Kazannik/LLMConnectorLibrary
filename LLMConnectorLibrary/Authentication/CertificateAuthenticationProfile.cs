using GigaChat;
using OpenAI;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace LLMConnectorLibrary.Authentication
{
	public class CertificateAuthenticationProfile<T> : AuthenticationProfile, ICertificateAuthenticationProfile<T>
		where T : class
	{		
		private CertificateAuthenticationProfile(IClientOptions clientOptions, X509Certificate2 certificate) 
			 : base(
				   clientOptions: clientOptions,
				   httpClient: CreateHttpClient(certificate: certificate, timeout: clientOptions.Timeout, baseAddress: clientOptions.Endpoint)
				   )
		{
			ApiType = GetApiType();
			LLMBaseClient = CreateBaseClient(options: clientOptions, certificate: certificate);
			Certificate = certificate;
		}

		private CertificateAuthenticationProfile(string caption, IClientOptions clientOptions, X509Certificate2 certificate)
			: this(
				  clientOptions: clientOptions,
				  certificate: certificate)
		{
			Caption = caption;
		}

		public override AuthenticationTypeEnum AuthenticationType { get; } = AuthenticationTypeEnum.Certificate;

		public X509Certificate2 Certificate { get; }

		private static HttpClient CreateHttpClient(X509Certificate2 certificate, TimeSpan timeout, Uri baseAddress)
		{
			var handler = new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				SslProtocols = SslProtocols.Tls12,
				CheckCertificateRevocationList = false,
				ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
			};
			
			if (certificate != null)
				handler.ClientCertificates.Add(certificate);

			return new HttpClient(handler)
			{
				Timeout = timeout,
				BaseAddress = baseAddress
			};
		}

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
		protected static T CreateBaseClient(IClientOptions options, X509Certificate2 certificate)
		{
			Type type = typeof(T);
			if (type.Name.Equals(nameof(OpenAIClient)))
			{
				return OpenAIService.CreateOpenAIClient(options: options, certificate: certificate) as T;
			}
			else if (type.Name.Equals(nameof(GigaChatClient)))
			{
				return GigaChatService.CreateGigaChatClient(options: options, certificate: certificate) as T;
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

		T ICertificateAuthenticationProfile<T>.LLMBaseClient => (T)LLMBaseClient;

		public override ApiTypeEnum ApiType { get; }
		public override IAuthenticationProfile Clone(IClientOptions clientOptions)
			=> new CertificateAuthenticationProfile<T>(caption: Caption, clientOptions: clientOptions, certificate: Certificate);
		public override IAuthenticationProfile Clone(int pingTimeout, TimeSpan timeout, Uri endpoint)
			=> new CertificateAuthenticationProfile<T>(caption: Caption, clientOptions: new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint), certificate: Certificate);

		public static IAuthenticationProfile Create(string caption, int pingTimeout, TimeSpan timeout, Uri endpoint, X509Certificate2 certificate)
			=> new CertificateAuthenticationProfile<T>(caption: caption, clientOptions: new ClientOptions(pingTimeout: pingTimeout, timeout: timeout, endpoint: endpoint), certificate: certificate);

		public static IAuthenticationProfile Create(IClientOptions clientOptions, X509Certificate2 certificate)
			=> new CertificateAuthenticationProfile<T>(clientOptions: clientOptions, certificate: certificate);
	}
}
