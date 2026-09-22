using LLMConnectorLibrary.Authentication;
using System;

namespace LLMConnectorLibrary.Models
{
	public interface IModel : IComparable<IModel>, IEquatable<IModel>
	{
		string Id { get; }
		string OwnedBy { get; }
		IAuthenticationProfile Profile { get; }
	}
}
