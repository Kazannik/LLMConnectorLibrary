namespace LLMConnectorLibrary.Authentication
{
	/// <summary>
	/// Типы авторизации.
	/// </summary>
	public enum AuthenticationTypeEnum : int
	{
		/// <summary>
		/// Отсутствие авторизации.
		/// </summary>
		Default = 0,
		/// <summary>
		/// Авторизация на основе токена.
		/// </summary>
		AuthorizationBearer = 1,
		/// <summary>
		/// Авторизация на основе токена, предназначенного для получения временных токенов.
		/// </summary>
		PreAuthentication = 2,
		/// <summary>
		/// Авторизация на основе сертификата.
		/// </summary>
		Certificate = 3
	}
}
