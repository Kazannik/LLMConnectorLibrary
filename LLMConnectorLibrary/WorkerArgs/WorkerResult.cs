using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerResult(
		MessageType type,
		string model,
		string systemMessage,
		IEnumerable<string> userMessages,
		string input,
		string message,
		ReadOnlyMemory<float> embedding,
		Exception exception,
		object tag)
	{
		public readonly MessageType MessageType = type;
		public readonly string Model = model;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly string Input = input;
		public readonly string Message = message;
		public readonly ReadOnlyMemory<float> Embedding = embedding;
		public readonly Exception Exception = exception;
		public readonly object Tag = tag;

		public WorkerResult(
			string model,
			string systemMessage,
			IEnumerable<string> userMessages,
			string message,
			object tag) : this(
				type: MessageType.Chat,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				input: string.Empty,
				message: message,
				embedding: null,
				exception: null,
				tag: tag)
		{
		}

		public WorkerResult(
			string model,
			string systemMessage,
			IEnumerable<string> userMessages,
			Exception exception,
			object tag) : this(
				type: MessageType.Chat,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				input: string.Empty,
				message: string.Empty,
				embedding: null,
				exception: exception,
				tag: tag)
		{
		}

		public WorkerResult(
			string model,
			string input,
			ReadOnlyMemory<float> embedding,
			object tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				input: input,
				message: string.Empty,
				embedding: embedding,
				exception: null,
				tag: tag)
		{
		}

		public WorkerResult(
			string model,
			string input,
			Exception exception,
			object tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				input: input,
				message: string.Empty,
				embedding: null,
				exception: exception,
				tag: tag)
		{
		}
	}
}
