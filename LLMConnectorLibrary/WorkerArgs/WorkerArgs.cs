// Ignore Spelling: Uri

using LLMConnectorLibrary.Authentication;
using LLMConnectorLibrary.Models;
using System.Collections.Generic;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerArgs(
		MessageType type,
		IAuthenticationProfile profile,
		IModel model,
		IChatOptions options,
		string systemMessage,
		IEnumerable<string> userMessages,
		IEnumerable<(int key, string description)> store,
		object? tag)
	{
		public readonly MessageType MessageType = type;
		public readonly IAuthenticationProfile Profile = profile;
		public readonly IModel Model = model;
		public readonly IChatOptions Options = options;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly IEnumerable<(int key, string description)> Store = store;
		public readonly object? Tag = tag;

		public WorkerArgs(
			IAuthenticationProfile profile,
			bool getModels,
			object? tag) : this(
				type: getModels ? MessageType.GetModels : MessageType.Available,
				profile: profile,
				model: null,
				options: ChatOptions.Empty,
				systemMessage: string.Empty,
				userMessages: [],
				store: [],
				tag: tag)
		{
		}

		public WorkerArgs(
			IModel model,
			IChatOptions options,
			string systemMessage,
			IEnumerable<string> userMessages,
			object? tag) : this(
				type: MessageType.Chat,
				profile: model.Profile,
				model: model,
				options: options,
				systemMessage: systemMessage,
				userMessages: userMessages,
				store: [],
				tag: tag)
		{
		}

		public WorkerArgs(
			IModel model,
			IEnumerable<(int key, string description)> store,
			object? tag) : this(
				type: MessageType.Embedding,
				profile: model.Profile,
				model: model,
				options: ChatOptions.Empty,
				systemMessage: string.Empty,
				userMessages: [],
				store: store,
				tag: tag)
		{
		}
	}
}
