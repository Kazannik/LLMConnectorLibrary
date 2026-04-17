// Ignore Spelling: Uri

using System;
using System.Collections.Generic;
using static LLMConnectorLibrary.LLMOpenAI;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerArgs(
		MessageType type,
		Uri uri,
		TimeSpan timeout,
		string model,
		ChatOptions options,
		string systemMessage,
		IEnumerable<string> userMessages,
		IEnumerable<(int key, string description)> store,
		object? tag)
	{
		public readonly MessageType MessageType = type;
		public readonly Uri Uri = uri;
		public readonly TimeSpan Timeout = timeout;
		public readonly string Model = model;
		public readonly ChatOptions Options = options;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly IEnumerable<(int key, string description)> Store = store;
		public readonly object? Tag = tag;

		public WorkerArgs(
			Uri uri,
			TimeSpan timeout,
			bool getModels,
			object? tag) : this(
			type: getModels ? MessageType.GetModels : MessageType.Available,
			uri: uri,
			timeout: timeout,
			model: string.Empty,
			options: ChatOptions.Empty,
			systemMessage: string.Empty,
			userMessages: [],
			store: [],
			tag: tag)
		{
		}

		public WorkerArgs(
			Uri uri,
			TimeSpan timeout,
			string model,
			ChatOptions options,
			string systemMessage,
			IEnumerable<string> userMessages,
			object? tag) : this(
				type: MessageType.Chat,
				uri: uri,
				timeout: timeout,
				model: model,
				options: options,
				systemMessage: systemMessage,
				userMessages: userMessages,
				store: [],
				tag: tag)
		{
		}

		public WorkerArgs(
			Uri uri,
			TimeSpan timeout,
		string model,
		IEnumerable<(int key, string description)> store,
		object? tag) : this(
			type: MessageType.Embedding,
			uri: uri,
			timeout: timeout,
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
