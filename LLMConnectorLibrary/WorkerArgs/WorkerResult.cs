// Ignore Spelling: Uri

using LLMConnectorLibrary.Authentication;
using LLMConnectorLibrary.Models;
using System;
using System.Collections.Generic;

namespace LLMConnectorLibrary.WorkerArgs
{
	internal readonly struct WorkerResult(
		MessageType type,
		IModel model,
		string systemMessage,
		IEnumerable<string> userMessages,
		IEnumerable<(int key, string description)> store,
		string message,
		IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> embedding,
		Exception? exception,
		IAuthenticationProfile profile,
		bool isAvailable,
		ModelsCollection models,
		object? tag)
	{
		public readonly MessageType MessageType = type;
		public readonly IModel Model = model;
		public readonly string SystemMessage = systemMessage;
		public readonly IEnumerable<string> UserMessages = userMessages;
		public readonly IEnumerable<(int key, string description)> Store = store;
		public readonly string Message = message;
		public readonly IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> Embedding = embedding;
		public readonly Exception? Exception = exception;
		public readonly IAuthenticationProfile Profile = profile;
		public readonly bool IsAvailable = isAvailable;
		public readonly ModelsCollection Models = models;
		public readonly object? Tag = tag;

		public WorkerResult(
			IAuthenticationProfile profile,
			bool isAvailable,
			object? tag) : this(
				type: MessageType.Available,
				model: null,
				systemMessage: string.Empty,
				userMessages: [],
				store: [],
				message: string.Empty,
				embedding: [],
				exception: null,
				profile: profile,
				isAvailable: isAvailable,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			IAuthenticationProfile profile,
			ModelsCollection models,
			bool isAvailable,
			object? tag) : this(
				type: MessageType.GetModels,
				model: null,
				systemMessage: string.Empty,
				userMessages: [],
				store: [],
				message: string.Empty,
				embedding: [],
				exception: null,
				profile: profile,
				isAvailable: isAvailable,
				models: models,
				tag: tag)
		{
		}

		public WorkerResult(
			IAuthenticationProfile profile,
			Exception? exception,
			object? tag) : this(
				type: MessageType.Embedding,
				model: null,
				systemMessage: string.Empty,
				userMessages: [],
				store: [],
				message: string.Empty,
				embedding: [],
				exception: exception,
				profile: profile,
				isAvailable: false,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			IModel model,
			string systemMessage,
			IEnumerable<string> userMessages,
			string message,
			object? tag) : this(
				type: MessageType.Chat,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				store: [],
				message: message,
				embedding: [],
				exception: null,
				profile: model.Profile,
				isAvailable: true,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			IModel model,
			string systemMessage,
			IEnumerable<string> userMessages,
			Exception? exception,
			object? tag) : this(
				type: MessageType.Chat,
				model: model,
				systemMessage: systemMessage,
				userMessages: userMessages,
				store: [],
				message: string.Empty,
				embedding: [],
				exception: exception,
				profile: model.Profile,
				isAvailable: false,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			IModel model,
			IEnumerable<(int key, string description)> store,
			IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> embedding,
			object? tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				store: store,
				message: string.Empty,
				embedding: embedding,
				exception: null,
				profile: model.Profile,
				isAvailable: true,
				models: [],
				tag: tag)
		{
		}

		public WorkerResult(
			IModel model,
			IEnumerable<(int key, string description)> store,
			Exception? exception,
			object? tag) : this(
				type: MessageType.Embedding,
				model: model,
				systemMessage: string.Empty,
				userMessages: [],
				store: store,
				message: string.Empty,
				embedding: [],
				exception: exception,
				profile: model.Profile,
				isAvailable: false,
				models: [],
				tag: tag)
		{
		}
	}
}
