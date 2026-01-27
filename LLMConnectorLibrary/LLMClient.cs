// Ignore Spelling: Uri

using LLMConnectorLibrary.EventArgs;
using LLMConnectorLibrary.WorkerArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Args = LLMConnectorLibrary.WorkerArgs.WorkerArgs;

namespace LLMConnectorLibrary
{
	public class LLMClient
	{
		#region Events

		public event EventHandler<ChatCompletedEventArgs> ChatCompleted;
		public event EventHandler<ChatCanceledEventArgs> ChatCanceled;
		public event EventHandler<ChatProgressEventArgs> ChatProgress;

		protected virtual void OnChatCompleted(ChatCompletedEventArgs e)
		{
			ChatCompleted?.Invoke(this, e);
		}

		protected virtual void OnChatCanceled(ChatCanceledEventArgs e)
		{
			ChatCanceled?.Invoke(this, e);
		}

		protected virtual void OnChatProgress(ChatProgressEventArgs e)
		{
			ChatProgress?.Invoke(this, e);
		}

		public event EventHandler<EmbedCompletedEventArgs> EmbedCompleted;
		public event EventHandler<EmbedCanceledEventArgs> EmbedCanceled;
		public event EventHandler<EmbedProgressEventArgs> EmbedProgress;

		protected virtual void OnEmbedCompleted(EmbedCompletedEventArgs e)
		{
			EmbedCompleted?.Invoke(this, e);
		}

		protected virtual void OnEmbedCanceled(EmbedCanceledEventArgs e)
		{
			EmbedCanceled?.Invoke(this, e);
		}

		protected virtual void OnEmbedProgress(EmbedProgressEventArgs e)
		{
			EmbedProgress?.Invoke(this, e);
		}

		#endregion

		private readonly BackgroundWorker llmWorker;

		public Uri Uri { get; }

		public LLMClient(Uri uri) //public AIClient(string url = "http://10.197.24.37:8000")  = "http://localhost:11434			
		{
			Uri = uri;

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

		public void Send(string model, IEnumerable<string> userMessages)
		{
			Send(model: model, systemMessage: string.Empty, userMessages: userMessages, tag: null);
		}

		public void Send(string model, string systemMessage, IEnumerable<string> userMessages)
		{
			Send(model: model, systemMessage: systemMessage, userMessages: userMessages, tag: null);
		}

		public void Send(string model, string systemMessage, IEnumerable<string> userMessages, object tag)
		{
			if (llmWorker.IsBusy != true)
			{
				Args args = new(Uri, model, systemMessage, userMessages, tag);
				llmWorker.RunWorkerAsync(args);
			}
		}

		public void Embed(string model, string input)
		{
			Embed(model: model, input: input, tag: null);
		}

		public void Embed(string model, string input, object tag)
		{
			if (llmWorker.IsBusy != true)
			{
				Args args = new(Uri, model, input, tag);
				llmWorker.RunWorkerAsync(args);
			}
		}

		private void LLM_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			Args argument = (Args)e.Argument;

			Uri uri = argument.Uri ?? new Uri(baseUri: new Uri("http://localhost:11434"), relativeUri: "/v1");

			Task<bool> isAvailable = Utils.Net.CheckHostByHttpAsync(new Uri(baseUri: uri, relativeUri: uri.AbsolutePath + "/models"));

			isAvailable.Wait();

			if (argument.MessageType == MessageType.Chat)
			{
				if (isAvailable.Result)
				{
					try
					{
						Task<string> message = LLMOpenAI.SendMessageAsync(
							uri: uri,
							model: argument.Model,
							systemMessage: argument.SystemMessage,
							userMessages: argument.UserMessages);

						message.Wait();

						e.Result = new WorkerResult(
							model: argument.Model,
							systemMessage: argument.SystemMessage,
							userMessages: argument.UserMessages,
							message: message.Result,
							tag: argument.Tag);
					}
					catch (Exception ex)
					{
						e.Result = new WorkerResult(
							model: argument.Model,
							systemMessage: argument.SystemMessage,
							userMessages: argument.UserMessages,
							exception: ex,
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
						tag: argument.Tag);
				}

				//while (result.Status == TaskStatus.Running)
				//{
				//	worker.ReportProgress(percentProgress: 0, userState: arguments);
				//	System.Threading.Thread.Sleep(50);
				//	if (worker.CancellationPending == true)
				//	{
				//		e.Cancel = true;
				//		break;
				//	}
				//}
			}
			else if (argument.MessageType == MessageType.Embedding)
			{
				if (isAvailable.Result)
				{
					try
					{
						Task<ReadOnlyMemory<float>> result = LLMOpenAI.GetEmbeddingAsync(
							uri: uri,
							model: argument.Model,
							input: argument.Input);

						result.Wait();

						e.Result = new WorkerResult(
							model: argument.Model,
							input: argument.Input,
							embedding: result.Result,
							tag: argument.Tag);
					}
					catch (Exception ex)
					{
						e.Result = new WorkerResult(
							model: argument.Model,
							input: argument.Input,
							exception: ex,
							tag: argument.Tag);
					}
				}
				else
				{
					e.Result = new WorkerResult(
						model: argument.Model,
						input: argument.Input,
						exception: new Exception(string.Format("Проверьте доступ к провайдеру по адресу: [{0}]", uri)),
						tag: argument.Tag);
				}


				//while (result.Status == TaskStatus.Running)
				//{
				//	worker.ReportProgress(percentProgress: 0, userState: arguments);
				//	System.Threading.Thread.Sleep(50);
				//	if (worker.CancellationPending == true)
				//	{
				//		e.Cancel = true;
				//		break;
				//	}
				//}
			}
		}

		private void LLM_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			WorkerResult result = (WorkerResult)e.Result;

			if (result.MessageType == MessageType.Chat)
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
						values: [result.Input],
						tag: result.Tag));
				}
				else if (e.Error != null || result.Exception != null)
				{
					OnEmbedCanceled(new EmbedCanceledEventArgs(
						model: result.Model,
						values: [result.Input],
						tag: result.Tag,
						cancel: false,
						error: true,
						exception: result.Exception));
				}
				else
				{
					OnEmbedCompleted(new EmbedCompletedEventArgs(
						model: result.Model,
						values: [result.Input],
						tag: result.Tag,
						embedding: [result.Embedding]));
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
					values: [state.Input],
					tag: state.Tag));
			}
		}
	}
}