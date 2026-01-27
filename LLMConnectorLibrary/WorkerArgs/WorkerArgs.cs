// Ignore Spelling: Uri

using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerArgs(
		MessageType type,
		Uri uri,
		string model,
		string systemMessage,
		IEnumerable<string> userMessages,
		string input,
		object tag)
	{
		public readonly MessageType MessageType = type;
		public readonly Uri Uri = uri;
		public readonly string Model = model;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly string Input = input;
		public readonly object Tag = tag;

		public WorkerArgs(
			Uri uri,
			string model,
			string systemMessage,
			IEnumerable<string> userMessages,
			object tag) : this(
				type: MessageType.Chat,
				uri: uri,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				input: string.Empty,
				tag: tag)
		{
		}

		public WorkerArgs(
			Uri uri,
		string model,
		string input,
		object tag) : this(
			type: MessageType.Embedding,
			uri: uri,
			model: model,
			systemMessage: string.Empty,
			userMessages: [],
			input: input,
			tag: tag)
		{
		}
	}
}
