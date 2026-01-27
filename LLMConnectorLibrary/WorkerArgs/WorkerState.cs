using System.Collections.Generic;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerState(
		MessageType type,
		string model,
		string systemMessage,
		IEnumerable<string> userMessages,
		string input,
		object tag)
	{
		public readonly MessageType MessageType = type;
		public readonly string Model = model;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly string Input = input;
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
				input: string.Empty,
				tag: tag)
		{
		}

		public WorkerState(
			string model,
			string input,
			object tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				input: input,
				tag: tag)
		{
		}
	}
}
