// Ignore Spelling: Uri

using LLMConnectorLibrary.EventArgs;
using LLMConnectorLibrary.Utils;
using LLMConnectorLibrary.WorkerArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using static LLMConnectorLibrary.LLMOpenAI;
using Args = LLMConnectorLibrary.WorkerArgs.WorkerArgs;

namespace LLMConnectorLibrary
{
	public class LLMClient
	{
		#region Events

		public event EventHandler<CheckHostEventArgs> HostChecked;

		protected virtual void OnHostChecked(CheckHostEventArgs e) => HostChecked?.Invoke(this, e);

		public event EventHandler<ModelsCollectionCompletedEventArgs> ModelsCollectionCompleted;

		protected virtual void OnModelsCollectionCompleted(ModelsCollectionCompletedEventArgs e) =>
			ModelsCollectionCompleted?.Invoke(this, e);

		public event EventHandler<ChatCompletedEventArgs> ChatCompleted;
		public event EventHandler<ChatCanceledEventArgs> ChatCanceled;
		public event EventHandler<ChatProgressEventArgs> ChatProgress;

		protected virtual void OnChatCompleted(ChatCompletedEventArgs e) => ChatCompleted?.Invoke(this, e);

		protected virtual void OnChatCanceled(ChatCanceledEventArgs e) => ChatCanceled?.Invoke(this, e);

		protected virtual void OnChatProgress(ChatProgressEventArgs e) => ChatProgress?.Invoke(this, e);

		public event EventHandler<EmbedCompletedEventArgs> EmbedCompleted;
		public event EventHandler<EmbedCanceledEventArgs> EmbedCanceled;
		public event EventHandler<EmbedProgressEventArgs> EmbedProgress;

		protected virtual void OnEmbedCompleted(EmbedCompletedEventArgs e) => EmbedCompleted?.Invoke(this, e);

		protected virtual void OnEmbedCanceled(EmbedCanceledEventArgs e) => EmbedCanceled?.Invoke(this, e);

		protected virtual void OnEmbedProgress(EmbedProgressEventArgs e) => EmbedProgress?.Invoke(this, e);

		#endregion

		private readonly BackgroundWorker llmWorker;

		public Uri Uri { get; }

		public TimeSpan Timeout { get; }

		public LLMClient(Uri uri, TimeSpan timeout) //public AIClient(string url = "http://10.197.24.37:8000")  = "http://localhost:11434			
		{
			Uri = uri;
			Timeout = timeout;

			llmWorker = new BackgroundWorker
			{
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			llmWorker.DoWork += new DoWorkEventHandler(LLM_DoWork);
			llmWorker.ProgressChanged += new ProgressChangedEventHandler(LLM_ProgressChanged);
			llmWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LLM_Completed);
		}


		//private async Task<string> SendAsync(string model, string systemMessage, string userMessage)
		//{
		//	//try
		//	//{
		//	//	ollamaClient.SelectedModel = model;
		//	//	Chat chat = new(ollamaClient, systemMessage);
		//	//	var answerBuilder = new StringBuilder();
		//	//	await foreach (string answerToken in chat.SendAsync(userMessage))
		//	//	{
		//	//		if (answerToken is null)
		//	//			continue;
		//	//		answerBuilder.Append(answerToken);
		//	//		await ollamaClient.IsRunningAsync();
		//	//	}
		//	//	return answerBuilder.ToString();
		//	//}

		//	////try
		//	////{
		//	////	ollamaClient.SelectedModel = model;
		//	////	Chat chat = new(ollamaClient, systemPrompt);
		//	////	string result = "";

		//	////	await foreach (string answerToken in chat.SendAsync(prompt))
		//	////		result += answerToken;
		//	////	return result;
		//	////}
		//	//catch (Exception ex)
		//	//{
		//	//	return ex.Message + " ИЛИ Ошибка в модуле AI: Превышен установленный временной интервал для подготовки информации.";
		//	//}
		//}

		//private async Task<IEnumerable<Embedding<float>>> EmbedAsync(string model, IEnumerable<string> values)
		//{
		//	ollamaClient.SelectedModel = model;
		//	IEmbeddingGenerator<string, Embedding<float>> generator = ollamaClient;
		//	return await generator.GenerateAsync(values);
		//}

		public void CheckHost(object? tag = default) => CheckHost(uri: Uri, timeout: Timeout, tag: tag);

		public void CheckHost(Uri uri, TimeSpan timeout, object? tag = default)
		{
			if (llmWorker.IsBusy != true)
			{
				Args args = new(uri: uri, timeout: timeout, getModels: false, tag: tag);
				llmWorker.RunWorkerAsync(args);
			}
		}

		public void ReadModelsName(object? tag = default) => ReadModelsName(uri: Uri, timeout: Timeout, tag: tag);

		public void ReadModelsName(Uri uri, TimeSpan timeout, object? tag = default)
		{
			if (llmWorker.IsBusy != true)
			{
				Args args = new(uri: uri, timeout: timeout, getModels: true, tag: tag);
				llmWorker.RunWorkerAsync(args);
			}
		}

		public void Send(string model, ChatOptions options, IEnumerable<string> userMessages)
		{
			Send(model: model, options: options, systemMessage: string.Empty, userMessages: userMessages);
		}
		
		public void Send(string model, ChatOptions options, string systemMessage, IEnumerable<string> userMessages, object? tag = default)
		{
			if (llmWorker.IsBusy != true)
			{
				Args args = new(uri: Uri, timeout: Timeout, model: model, options: options, systemMessage: systemMessage, userMessages: userMessages, tag: tag);
				llmWorker.RunWorkerAsync(args);
			}
		}

		public void Embed(string model, string input, object? tag)
		{
			if (llmWorker.IsBusy != true)
			{
				Args args = new(Uri, Timeout, model, [(1, input)], tag);
				llmWorker.RunWorkerAsync(args);
			}
		}

		public void Embed(string model, IEnumerable<(int key, string description)> store, object? tag)
		{
			if (llmWorker.IsBusy != true)
			{
				Args args = new(Uri, Timeout, model, store, tag);
				llmWorker.RunWorkerAsync(args);
			}
		}

		public void Cancel()
		{
			if (llmWorker.WorkerSupportsCancellation)
			{
				llmWorker.CancelAsync();
			}
		}

		private void LLM_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker? worker = sender as BackgroundWorker;

			if (worker?.CancellationPending == true)
			{
				e.Cancel = true;
				return;
			}

			Args argument = (Args)e.Argument;
			Uri uri = argument.Uri ?? new Uri("http://localhost:11434");
			TimeSpan timeout = argument.Timeout;

			bool isAvailable = Net.CheckHostByHttp(uri);

			if (argument.MessageType == MessageType.Available)
			{
				e.Result = new WorkerResult(uri: uri, timeout: timeout, isAvailable: isAvailable, tag: argument.Tag);
			}
			else if (argument.MessageType == MessageType.GetModels)
			{
				if (isAvailable)
				{
					IEnumerable<string> models = LLMOpenAI.GetModelsName(uri, timeout);
					e.Result = new WorkerResult(uri: uri, timeout: timeout, models: models, isAvailable: isAvailable, tag: argument.Tag);
				}
				else
				{
					e.Result = new WorkerResult(uri: uri, timeout: timeout, models: [], isAvailable: isAvailable, tag: argument.Tag);
				}
			}
			else if (argument.MessageType == MessageType.Chat)
			{
				if (isAvailable)
				{
					try
					{

#if AI_MESSAGE_DEBUG
						string message = string.Format("Model: {0};\nTime: {1};\nSystem Message: {2};\nUser Messages:\n{3}", argument.Model, DateTime.Now, argument.SystemMessage, string.Join("\n", argument.UserMessages));
#else

						string message = LLMOpenAI.SendMessageAsync(
							uri: uri,
							timeout: timeout,
							model: argument.Model,
							options: argument.Options,
							systemMessage: argument.SystemMessage,
							userMessages: argument.UserMessages)
							.GetAwaiter().GetResult();
#endif

						e.Result = new WorkerResult(
							model: argument.Model,
							systemMessage: argument.SystemMessage,
							userMessages: argument.UserMessages,
							message: message,
							uri: uri,
							timeout: timeout,
							tag: argument.Tag);
					}
					catch (Exception ex)
					{
						e.Result = new WorkerResult(
							model: argument.Model,
							systemMessage: argument.SystemMessage,
							userMessages: argument.UserMessages,
							exception: ex,
							uri: uri,
							timeout: timeout,
							tag: argument.Tag);
					}
				}
				else
				{
					e.Result = new WorkerResult(
						model: argument.Model,
						systemMessage: argument.SystemMessage,
						userMessages: argument.UserMessages,
						exception: new Exception(string.Format("Проверьте доступ к провайдеру по адресу: [{0}]", uri)),
						uri: uri,
						timeout: timeout,
						tag: argument.Tag);
				}
			}
			else if (argument.MessageType == MessageType.Embedding)
			{
				if (isAvailable)
				{
					try
					{
						IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> result = LLMOpenAI.GetEmbeddingAsync(
							uri: uri,
							timeout: timeout,
							model: argument.Model,
							store: argument.Store,
							ProgressChanged)
							.GetAwaiter().GetResult();

						e.Result = new WorkerResult(
							model: argument.Model,
							store: argument.Store,
							embedding: result,
							uri: uri,
							timeout: timeout,
							tag: argument.Tag);
					}
					catch (Exception ex)
					{
						e.Result = new WorkerResult(
							model: argument.Model,
							store: argument.Store,
							exception: ex,
							uri: uri,
							timeout: timeout,
							tag: argument.Tag);
					}
				}
				else
				{
					e.Result = new WorkerResult(
						model: argument.Model,
						store: argument.Store,
						exception: new Exception(string.Format("Проверьте доступ к провайдеру по адресу: [{0}]", uri)),
						uri: uri,
						timeout: timeout,
						tag: argument.Tag);
				}
			}
		}

		private void LLM_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			WorkerResult result = (WorkerResult)e.Result;

			if (result.MessageType == MessageType.Available)
			{
				OnHostChecked(new CheckHostEventArgs(uri: result.Uri, timeout: result.Timeout, isAvailable: result.IsAvailable, tag: result.Tag));
			}
			else if (result.MessageType == MessageType.GetModels)
			{
				OnModelsCollectionCompleted(new ModelsCollectionCompletedEventArgs(
					uri: result.Uri,
					models: result.Models,
					tag: result.Tag));
			}
			else if (result.MessageType == MessageType.Chat)
			{
				if (e.Cancelled == true)
				{
					OnChatCanceled(new ChatCanceledEventArgs(
						model: result.Model,
						systemMessage: result.SystemMessage,
						userMessages: result.UserMessages,
						tag: result.Tag));
				}
				else if (e.Error != null || result.Exception != null)
				{
					OnChatCanceled(new ChatCanceledEventArgs(
						model: result.Model,
						systemMessage: result.SystemMessage,
						userMessages: result.UserMessages,
						tag: result.Tag,
						cancel: false,
						error: true,
						exception: result.Exception));
				}
				else
				{
					OnChatCompleted(new ChatCompletedEventArgs(
						model: result.Model,
						systemMessage: result.SystemMessage,
						userMessages: result.UserMessages,
						tag: result.Tag,
						message: result.Message));
				}
			}
			else if (result.MessageType == MessageType.Embedding)
			{
				if (e.Cancelled == true)
				{
					OnEmbedCanceled(new EmbedCanceledEventArgs(
						model: result.Model,
						values: result.Store,
						tag: result.Tag));
				}
				else if (e.Error != null || result.Exception != null)
				{
					OnEmbedCanceled(new EmbedCanceledEventArgs(
						model: result.Model,
						values: result.Store,
						tag: result.Tag,
						cancel: false,
						error: true,
						exception: result.Exception));
				}
				else
				{
					OnEmbedCompleted(new EmbedCompletedEventArgs(
						model: result.Model,
						values: result.Store,
						tag: result.Tag,
						embedding: result.Embedding));
				}
			}
		}

		private void LLM_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			WorkerState state = (WorkerState)e.UserState;
			if (state.MessageType == MessageType.Chat)
			{
				OnChatProgress(new ChatProgressEventArgs(
					model: state.Model,
					systemMessage: state.SystemMessage,
					userMessages: state.UserMessages,
					tag: state.Tag));
			}
			else if (state.MessageType == MessageType.Embedding)
			{
				OnEmbedProgress(new EmbedProgressEventArgs(
					model: state.Model,
					values: state.Store,
					tag: state.Tag));
			}
		}

		private void ProgressChanged(int received, int totalToReceive, int progressPercentage)
		{
			Debug.WriteLine(received);
		}
	}
}