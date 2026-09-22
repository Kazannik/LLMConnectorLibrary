using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLMConnectorLibrary.Authentication
{
	public class AuthenticationProfileCollection : CollectionBase, IEnumerable<IAuthenticationProfile>
	{
		public AuthenticationProfileCollection() { }

		public AuthenticationProfileCollection(IEnumerable<IAuthenticationProfile> profiles) : this()
		{
			foreach (IAuthenticationProfile profile in profiles)
			{
				Add(profile: profile);
			}
		}

		public async Task CheckStateAsync()
		{
			try
			{
				foreach (AuthenticationProfileEntity entity in List)
				{
					await entity.CheckStateAsync();
				}
			}
			catch (Exception) { }			
		}

		public IEnumerable<IAuthenticationProfile> ConnectedProfiles()
		{
			return List.Cast<AuthenticationProfileEntity>()
				.Where(x => x.IsConnected)
				.Select(x => x.Profile);
		}

		private bool Contains(IAuthenticationProfile profile)
		{
			return this.Any(x => x.ClientOptions.Endpoint == profile.ClientOptions.Endpoint);
		}

		public void Add(IAuthenticationProfile profile)
		{
			if (!Contains(profile))
				List.Add(new AuthenticationProfileEntity(profile));
		}

		public void AddRange(IEnumerable<IAuthenticationProfile> profiles)
		{
			foreach (IAuthenticationProfile profile in profiles)
			{
				Add(profile: profile);
			}
		}

		public void AddRange(AuthenticationProfileCollection profiles) => AddRange(profiles: profiles);

		public IAuthenticationProfile this[int index] => ((AuthenticationProfileEntity)List[index: index]).Profile;

		IEnumerator<IAuthenticationProfile> IEnumerable<IAuthenticationProfile>.GetEnumerator()
		{
			return new AuthenticationProfileEnum(List);
		}

		private class AuthenticationProfileEntity(IAuthenticationProfile profile)
		{
			public IAuthenticationProfile Profile { get; } = profile;

			public bool IsPing { get; private set; }

			public bool IsConnected { get; private set; }

			public async Task CheckStateAsync()
			{
				if (Profile is IPreAuthenticationProfile<OpenAI.OpenAIClient> openAiProfile)
				{
					IsPing = (await openAiProfile.OauthPingAsync().ConfigureAwait(false))
						&& (await openAiProfile.PingAsync().ConfigureAwait(false));

					IsConnected = (0 < (await openAiProfile.CheckOauthHostAsync().ConfigureAwait(false)))
						&& (0 < (await openAiProfile.CheckHostAsync().ConfigureAwait(false)));
				}
				else if (Profile is IPreAuthenticationProfile<GigaChat.GigaChatClient> gigaChatProfile)
				{
					IsPing = (await gigaChatProfile.OauthPingAsync().ConfigureAwait(false))
						&& (await gigaChatProfile.PingAsync().ConfigureAwait(false));

					IsConnected = (0 < (await gigaChatProfile.CheckOauthHostAsync().ConfigureAwait(false)))
						&& (0 < (await gigaChatProfile.CheckHostAsync().ConfigureAwait(false)));
				}
				else
				{
					IsPing = await Profile.PingAsync().ConfigureAwait(false);
					IsConnected = 0 < await Profile.CheckHostAsync().ConfigureAwait(false);
				}				
			}
		}

		private class AuthenticationProfileEnum(IList list) : IEnumerator<IAuthenticationProfile>
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

			public IAuthenticationProfile Current
			{
				get
				{
					try
					{
						return ((AuthenticationProfileEntity)list[position]).Profile;
					}
					catch (IndexOutOfRangeException)
					{
						throw new InvalidOperationException();
					}
				}
			}

			IAuthenticationProfile IEnumerator<IAuthenticationProfile>.Current => Current;
		}
	}
}
