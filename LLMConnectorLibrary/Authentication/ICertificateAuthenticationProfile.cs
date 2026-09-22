using System.Security.Cryptography.X509Certificates;

namespace LLMConnectorLibrary.Authentication
{
	public interface ICertificateAuthenticationProfile<T> : IAuthenticationProfile
		where T : class
	{
		X509Certificate2 Certificate { get; }
		new T LLMBaseClient { get; }
	}
}
