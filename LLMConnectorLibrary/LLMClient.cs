// Ignore Spelling: Uri

using LLMConnectorLibrary.Authentication;
using LLMConnectorLibrary.EventArgs;
using LLMConnectorLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LLMConnectorLibrary
{
	public partial class LLMClient
	{
		private static readonly SemaphoreSlim semaphore = new(10);

		public async Task CheckHostAsync(IAuthenticationProfile profile, object? tag = default)
		{
			bool isPing = false;
			await semaphore.WaitAsync();
			try
			{
				#if AI_MESSAGE_DEBUG
				await Task.Factory.StartNew(() =>
				{
					Thread.Sleep(500);
					isPing = true;
				});
				#else
				isPing = profile.ClientOptions.PingTimeout == 0 || await profile.PingAsync().ConfigureAwait(false);
				#endif
			}
			catch (Exception ex) 
			{
				OnClientError(new ClientErrorEventArgs(ex));
			}
			finally
			{
				OnHostChecked(new CheckHostEventArgs(profile: profile, isAvailable: isPing, tag: tag));
				semaphore.Release();
			}
		}

		public async Task ReadModelsNameAsync(IAuthenticationProfile profile, object? tag = default)
		{
			ModelsCollection models = [];
			await semaphore.WaitAsync();

			try
			{
#if AI_MESSAGE_DEBUG
				await Task.Factory.StartNew(() =>
				{
					Thread.Sleep(500);
					models = [
						new Model("Test model 1 (debug)", "debug 1", AuthenticationProfileFactory.TestProfile),
						new Model("Test model 2 (debug)", "debug 2", AuthenticationProfileFactory.TestProfile)];
				});
#else
				if (profile.ApiType == ApiTypeEnum.GigaChat)
				{
					models = await GigaChatService.GetModelsAsync(profile);
				}
				else if (profile.ApiType == ApiTypeEnum.OpenAI)
				{
					models = await OpenAIService.GetModelsAsync(profile);
				}
				else
				{
					throw new NotImplementedException();
				}
				#endif
			}
			catch (Exception ex)
			{
				OnClientError(new ClientErrorEventArgs(ex));
			}
			finally
			{
				OnModelsCollectionCompleted(new ModelsCollectionCompletedEventArgs(profile: profile, models: models, tag: tag));
				semaphore.Release();
			}
		}

		public async Task SendAsync(IModel model, IChatOptions options, IEnumerable<string> userMessages) =>
			await SendAsync(model: model, options: options, systemMessage: string.Empty, userMessages: userMessages);

		public async Task SendAsync(IModel model, IChatOptions options, string systemMessage, IEnumerable<string> userMessages, object? tag = default)
		{
			await semaphore.WaitAsync();
			string message = string.Empty;

			try
			{
				#if AI_MESSAGE_DEBUG
				await Task.Factory.StartNew(() =>
				{
					Thread.Sleep(1000);
					message = string.Format("Model: {0};\nTime: {1};\nSystem Message: {2};\nUser Messages:\n{3}", model, DateTime.Now, systemMessage, string.Join("\n", userMessages));
				});
				#else
				if (model.Profile.ApiType == ApiTypeEnum.GigaChat)
				{
					message = await GigaChatService.SendMessageAsync(
						model: model,
						options: options,
						systemMessage: systemMessage,
						userMessages: userMessages);
				}
				else if (model.Profile.ApiType == ApiTypeEnum.OpenAI)
				{
					message = await OpenAIService.SendMessageAsync(
						model: model,
						options: options,
						systemMessage: systemMessage,
						userMessages: userMessages);
				}
				else
				{
					throw new NotImplementedException();
				}
				#endif
				if (message.EndsWith("\r\n")) message = message[..^2];
				if (message.EndsWith("\n\n")) message = message[..^2];
				if (message.EndsWith("\r")) message = message[..^1];
				if (message.EndsWith("\n")) message = message[..^1];
				message = message.Trim();
				OnChatCompleted(new ChatCompletedEventArgs(model: model, options: options, systemMessage: systemMessage, userMessages: userMessages, tag: tag, message: message));
			}
			catch (Exception ex)
			{
				OnChatCanceled(new ChatCanceledEventArgs(model: model, options: options, systemMessage: systemMessage, userMessages: userMessages, tag: tag, cancel: false, error: true, exception: ex));
			}
			finally { semaphore.Release(); }
		}

		public async Task EmbedAsync(IModel model, string input, object? tag) => await EmbedAsync(model: model, store: [(1, input)], tag: tag);

		public async Task EmbedAsync(IModel model, IEnumerable<(int key, string description)> store, object? tag)
		{
			await semaphore.WaitAsync();
			IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)> embedding = [];

			try
			{
				if (model.Profile.ApiType == ApiTypeEnum.GigaChat)
				{
					embedding = await GigaChatService.GetEmbeddingAsync(
						model: model,
						store: store,
						ProgressChanged);
				}
				else if (model.Profile.ApiType == ApiTypeEnum.OpenAI)
				{
					embedding = await OpenAIService.GetEmbeddingAsync(
						model: model,
						store: store,
						ProgressChanged);
				}
				else
				{
					throw new NotImplementedException();
				}
				OnEmbedCompleted(new EmbedCompletedEventArgs(
						model: model,
						values: store,
						tag: tag,
						embedding: embedding));
			}
			catch (Exception ex)
			{
				OnEmbedCanceled(new EmbedCanceledEventArgs(
						model: model,
						values: store,
						tag: tag,
						cancel: false,
						error: true,
						exception: ex));
			}
			finally { semaphore.Release(); }
		}

		private void ProgressChanged(int received, int totalToReceive, int progressPercentage)
		{
			Debug.WriteLine(received);
		}		
	}
}