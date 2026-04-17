// Ignore Spelling: Uri

using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerResult(
		MessageType type,
		string model,
		string systemMessage,
		IEnumerable<string> userMessages,
		IEnumerable<(int key, string description)> store,
		string message,
		IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> embedding,
		Exception? exception,
		Uri uri,
		TimeSpan timeout,
		bool isAvailable,
		IEnumerable<string> models,
		object? tag)
	{
		public readonly MessageType MessageType = type;
		public readonly string Model = model;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly IEnumerable<(int key, string description)> Store = store;
		public readonly string Message = message;
		public readonly IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> Embedding = embedding;
		public readonly Exception? Exception = exception;
		public readonly Uri Uri = uri;
		public readonly TimeSpan Timeout = timeout;
		public readonly bool IsAvailable = isAvailable;
		public readonly IEnumerable<string> Models = models;
		public readonly object? Tag = tag;

		public WorkerResult(
			Uri uri,
			TimeSpan timeout,
			bool isAvailable,
			object? tag) : this(
				type: MessageType.Available,
				model: string.Empty,
				systemMessage: string.Empty,
				userMessages: [],
				store: [],
				message: string.Empty,
				embedding: [],
				exception: null,
				uri: uri,
				timeout: timeout,
				isAvailable: isAvailable,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			Uri uri,
			TimeSpan timeout,
			IEnumerable<string> models,
			bool isAvailable,
			object? tag) : this(
				type: MessageType.GetModels,
				model: string.Empty,
				systemMessage: string.Empty,
				userMessages: [],
				store: [],
				message: string.Empty,
				embedding: [],
				exception: null,
				uri: uri,
				timeout: timeout,
				isAvailable: isAvailable,
				models: models,
				tag: tag)
		{
		}

		public WorkerResult(
			string model,
			string systemMessage,
			IEnumerable<string> userMessages,
			string message,
			Uri uri,
			TimeSpan timeout,
			object? tag) : this(
				type: MessageType.Chat,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				store: [],
				message: message,
				embedding: [],
				exception: null,
				uri: uri,
				timeout: timeout,
				isAvailable: true,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			string model,
			string systemMessage,
			IEnumerable<string> userMessages,
			Exception? exception,
			Uri uri,
			TimeSpan timeout,
			object? tag) : this(
				type: MessageType.Chat,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				store: [],
				message: string.Empty,
				embedding: [],
				exception: exception,
				uri: uri,
				timeout: timeout,
				isAvailable: false,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			string model,
			IEnumerable<(int key, string description)> store,
			IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> embedding,
			Uri uri,
			TimeSpan timeout,
			object? tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				store: store,
				message: string.Empty,
				embedding: embedding,
				exception: null,
				uri: uri,
				timeout: timeout,
				isAvailable: true,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			string model,
			IEnumerable<(int key, string description)> store,
			Exception? exception,
			Uri uri,
			TimeSpan timeout,
			object? tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				store: store,
				message: string.Empty,
				embedding: [],
				exception: exception,
				uri: uri,
				timeout: timeout,
				isAvailable: false,
				models: [],
				tag: tag)
		{
		}
	}
}
