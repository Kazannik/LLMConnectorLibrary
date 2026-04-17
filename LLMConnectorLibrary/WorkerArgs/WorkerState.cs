using System.Collections.Generic;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerState(
		MessageType type,
		string model,
		string systemMessage,
		IEnumerable<string> userMessages,
		IEnumerable<(int key, string description)> store,
		object tag)
	{
		public readonly MessageType MessageType = type;
		public readonly string Model = model;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly IEnumerable<(int key, string description)> Store = store;
		public readonly object Tag = tag;

		public WorkerState(
			string model,
			string systemMessage,
			IEnumerable<string> userMessages,
			object tag) : this(
				type: MessageType.Chat,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				store: [],
				tag: tag)
		{
		}

		public WorkerState(
			string model,
			IEnumerable<(int key, string description)> store,
			object tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				store: store,
				tag: tag)
		{
		}
	}
}
