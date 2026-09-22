using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace LLMConnectorLibrary
{
	public static class NetService
	{
		public static HttpClientHandler CreateHttpClientHandler(X509Certificate2 certificate)
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

			return handler;
		}
	}
}
