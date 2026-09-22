using LLMConnectorLibrary.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLMConnectorLibrary.Models
{
	public class ModelsCollection : CollectionBase, IEnumerable<IModel>
	{
		public static ModelsCollection Create(IAuthenticationProfile profile, OpenAI.Models.OpenAIModelCollection models)
		{
			return new ModelsCollection(profile, models);
		}

		public static ModelsCollection Create(IAuthenticationProfile profile, GigaChat.Models.GigaChatModelCollection models)
		{
			return new ModelsCollection(profile, models);
		}

		public ModelsCollection() { }

		private ModelsCollection(IAuthenticationProfile profile, OpenAI.Models.OpenAIModelCollection models)
		{
			foreach (OpenAI.Models.OpenAIModel model in models)
			{
				Add(id: model.Id, ownedBy: model.OwnedBy, profile: profile);
			}
		}

		private ModelsCollection(IAuthenticationProfile profile, GigaChat.Models.GigaChatModelCollection models)
		{
			foreach (GigaChat.Models.GigaChatModel model in models)
			{
				Add(id: model.Id, ownedBy: model.OwnedBy, profile: profile);
			}
		}

		private bool Contains(IModel model)
		{
			return this.Any(x => x.Profile.Equals(model.Profile) && x.Id == model.Id);
		}

		public void Add(IModel model)
		{
			if (!Contains(model))
				List.Add(model);
		}

		public void Add(string id, string ownedBy, IAuthenticationProfile profile) => Add(new Model(id: id, ownedBy: ownedBy, profile: profile));

		public void AddRange(IAuthenticationProfile profile, OpenAI.Models.OpenAIModelCollection models)
		{
			foreach (OpenAI.Models.OpenAIModel model in models)
			{
				Add(id: model.Id, ownedBy: model.OwnedBy, profile: profile);
			}
		}

		public void AddRange(IAuthenticationProfile profile, GigaChat.Models.GigaChatModelCollection models)
		{
			foreach (GigaChat.Models.GigaChatModel model in models)
			{
				Add(id: model.Id, ownedBy: model.OwnedBy, profile: profile);
			}
		}

		public void AddRange(ModelsCollection models)
		{
			foreach (IModel model in models)
			{
				Add(model: model);
			}
		}

		public IModel this[int index] => (IModel)List[index: index];

		public void Remove(IModel model)
		{
			if (Contains(model)) List.Remove(model);
		}

		IEnumerator<IModel> IEnumerable<IModel>.GetEnumerator()
		{
			return new ModelEnum(List);
		}

		private class ModelEnum(IList list) : IEnumerator<IModel>
		{
			private readonly IList list = list;

			int position = -1;

			public bool MoveNext()
			{
				position++;
				return position < list.Count;
			}

			public void Reset() => position = -1;

			public void Dispose()
			{
				//throw new NotImplementedException();
			}

			object IEnumerator.Current => Current;

			public IModel Current
			{
				get
				{
					try
					{
						return (IModel)list[position];
					}
					catch (IndexOutOfRangeException)
					{
						throw new InvalidOperationException();
					}
				}
			}

			IModel IEnumerator<IModel>.Current => Current;
		}
	}
}
