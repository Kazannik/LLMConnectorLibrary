namespace LLMConnectorLibrary.Authentication
{
	public interface IDefaultAuthenticationProfile<T> : IAuthenticationProfile
		where T : class
	{
		new T LLMBaseClient { get; }
	}
}
