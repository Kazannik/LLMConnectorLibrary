using LLMConnectorLibrary.Authentication;
using System;

namespace LLMConnectorLibrary.Models
{
	public class Model(string id, string ownedBy, IAuthenticationProfile profile) : IModel
	{
		public string Id { get; } = id;
		public string OwnedBy { get; } = ownedBy;
		public IAuthenticationProfile Profile { get; } = profile;

		public int CompareTo(IModel other)
		{
			if (other == null) return 1;
			if (other is IModel model)
				return Id.CompareTo(model.Id);
			else
				throw new ArgumentException("Object is not a IModel");
		}

		public bool Equals(IModel other)
		{
			if (other == null || other is not IModel model)
				return false;
			else
				return Id == model.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Profile.ClientOptions.Endpoint.GetHashCode();
		}

		public override string ToString()
		{
			return Id;
		}
	}
}
