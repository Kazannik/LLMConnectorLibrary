using System;

namespace LLMConnectorLibrary.Authentication
{
	public interface IAuthorizationBearerAuthenticationProfile<T> : IAuthenticationProfile
		where T : class
	{
		string AccessToken { get; }

		TimeSpan AccessTokenLifeTime { get; }

		new T LLMBaseClient { get; }
	}
}
