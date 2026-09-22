// Ignore Spelling: uri OPENAI Llm

using GigaChat;
using LLMConnectorLibrary.Authentication;
using LLMConnectorLibrary.Models;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Models;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatFinishReason = OpenAI.Chat.ChatFinishReason;
using ChatMessage = OpenAI.Chat.ChatMessage;


namespace LLMConnectorLibrary
{
	public static class OpenAIService
	{
		private const string DEFAULT_OPENAI_API_KEY = "OPENAI_API_KEY";

		public static OpenAIClient CreateOpenAIClient(IClientOptions options, string key = DEFAULT_OPENAI_API_KEY)
		{
			OpenAIClientOptions clientOptions = new()
			{
				Endpoint = options.Endpoint,
				NetworkTimeout = options.Timeout,
			};
			return new(new ApiKeyCredential(key), clientOptions);
		}

		public static OpenAIClient CreateOpenAIClient(IClientOptions options, X509Certificate2 certificate, string key = DEFAULT_OPENAI_API_KEY)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

			HttpClientHandler handler = NetService.CreateHttpClientHandler(certificate: certificate);
			
			HttpClient httpClient = new(handler);
			
			OpenAIClientOptions clientOptions = new()
			{
				Transport = new HttpClientPipelineTransport(httpClient),
				RetryPolicy = new ClientRetryPolicy(maxRetries: 0),
				Endpoint = options.Endpoint,
				NetworkTimeout = options.Timeout,
			};
			return new(new ApiKeyCredential(key), clientOptions);
		}


		private static void AccessTokenValid(IAuthenticationProfile profile)
		{
			if (profile.AuthenticationType == AuthenticationTypeEnum.PreAuthentication)
			{
				IPreAuthenticationProfile<OpenAIClient> authenticationProfile = (IPreAuthenticationProfile<OpenAIClient>)profile;
				if (!authenticationProfile.AccessTokenIsValid)
				{
					// TODO AccessToken Refresh
				}
			}
		}

		public static IEnumerable<string> GetModelsName(IAuthenticationProfile profile)
		{
			return GetModels(profile: profile).Select(x => x.Id);
		}

		public static ModelsCollection GetModels(IAuthenticationProfile profile)
		{
			try
			{
				OpenAIModelCollection modelsResult = GetModelsCollection(profile: profile);
				return ModelsCollection.Create(profile, modelsResult);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public static async Task<ModelsCollection> GetModelsAsync(IAuthenticationProfile profile)
		{
			try
			{
				OpenAIModelCollection modelsResult = await GetModelsCollectionAsync(profile: profile);
				return ModelsCollection.Create(profile, modelsResult);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private static OpenAIModelCollection GetModelsCollection(IAuthenticationProfile profile)
		{
			OpenAIModelClient modelClient = GetModelClient(profile: profile);
			try
			{
				ClientResult<OpenAIModelCollection>? result = modelClient.GetModels();
				return result.Value;
			}
			catch (HttpRequestException ex)
			{
				throw new ApplicationException($"Ошибка при отправке HTTP запроса: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Неизвестная ошибка: {ex.Message}", ex);
			}
		}

		private async static Task<OpenAIModelCollection> GetModelsCollectionAsync(IAuthenticationProfile profile)
		{
			OpenAIModelClient modelClient = GetModelClient(profile: profile);
			try
			{
				var result = await modelClient.GetModelsAsync().ConfigureAwait(false);
				return result.Value;
			}
			catch (HttpRequestException ex)
			{
				throw new ApplicationException($"Ошибка при отправке HTTP запроса: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Неизвестная ошибка: {ex.Message}", ex);
			}
		}

		private static OpenAIModelClient GetModelClient(IAuthenticationProfile profile)
		{
			AccessTokenValid(profile: profile);

			OpenAIClient client = (OpenAIClient)profile.LLMBaseClient;

			return client.GetOpenAIModelClient();
		}

		public async static Task<string> SendMessageAsync(IModel model, IChatOptions options, string systemMessage, IEnumerable<string> userMessages)
		{
			AccessTokenValid(profile: model.Profile);

			OpenAIClient client = (OpenAIClient)model.Profile.LLMBaseClient;

			ChatClient chatClient = client.GetChatClient(model: model.Id);

			List<ChatMessage> chatMessages = [];

			if (!string.IsNullOrWhiteSpace(systemMessage))
				chatMessages.Add(new SystemChatMessage(systemMessage));

			foreach (string message in userMessages)
			{
				if (!string.IsNullOrWhiteSpace(message))
					chatMessages.Add(new UserChatMessage(message));
			}

			ClientResult<ChatCompletion> creativeWriterResult = await chatClient.CompleteChatAsync(
				chatMessages,
				new ChatCompletionOptions()
				{
					MaxOutputTokenCount = options.MaxOutputTokenCount,
					FrequencyPenalty = options.FrequencyPenalty,
					PresencePenalty = options.PresencePenalty,
					Temperature = options.Temperature,
					TopP = options.TopP,
				})
				.ConfigureAwait(false);
			return creativeWriterResult.Value.Content[0].Text;
		}

		public async static Task<ReadOnlyMemory<float>> GetEmbeddingAsync(IModel model, string input)
		{
			AccessTokenValid(profile: model.Profile);

			OpenAIClient client = (OpenAIClient)model.Profile.LLMBaseClient;

			IEmbeddingGenerator<string, Embedding<float>> generator = client
				.GetEmbeddingClient(model: model.Id)
				.AsIEmbeddingGenerator();

			return await generator.GenerateVectorAsync(input)
				.ConfigureAwait(false);
		}

		public static IEnumerable<string> TestEmbedding(IAuthenticationProfile profile, IEnumerable<string> models)
		{
			AccessTokenValid(profile: profile);

			OpenAIClient client = (OpenAIClient)profile.LLMBaseClient;

			List<string> result = [];
			foreach (string model in models)
			{
				IEmbeddingGenerator<string, Embedding<float>> generator = client
					.GetEmbeddingClient(model: model)
					.AsIEmbeddingGenerator(2);
				try
				{
					generator.GenerateVectorAsync("test").GetAwaiter().GetResult();
					result.Add(model);
				}
				catch (Exception) { }
				finally
				{
					generator.Dispose();
				}
			}
			return result;
		}

		public delegate void ProgressChanged(int received, int totalToReceive, int progressPercentage);

		public async static Task<IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)>> GetEmbeddingAsync(
			IModel model,
			IEnumerable<(int key, string description)> store,
			ProgressChanged progress)
		{
			AccessTokenValid(profile: model.Profile);

			OpenAIClient client = (OpenAIClient)model.Profile.LLMBaseClient;

			IEmbeddingGenerator<string, Embedding<float>> generator = client
				.GetEmbeddingClient(model: model.Id)
				.AsIEmbeddingGenerator();

			List<(int key, string description, ReadOnlyMemory<float> vector)> result = [];
			int received = 0, totalToReceive = store.Count();

			foreach (var (key, description) in store)
			{
				ReadOnlyMemory<float> vector = await generator.GenerateVectorAsync(description);
				result.Add(new(key, description, vector));
				received++;
				double percent = (double)received / totalToReceive * 100;
				progress.Invoke(received, totalToReceive, (int)percent);
			}
			return result;
		}

		public static void CreateMultipleClients()
		{
			OpenAIClientOptions options = new()
			{
				Endpoint = new Uri("")
			};

			OpenAIClient client = new(
				new ApiKeyCredential(DEFAULT_OPENAI_API_KEY),
				options);

			OpenAIModelClient modelClient = client.GetOpenAIModelClient();
			modelClient.GetModels();

			EmbeddingClient embeddingClient = client.GetEmbeddingClient("mistral");

			ChatClient chatClient = client.GetChatClient("mistral");

			ClientResult<ChatCompletion> creativeWriterResult = chatClient.CompleteChat(
			[
				new SystemChatMessage("Ты профессиональный юрист. Проводишь консультацию клиента. Не фантазируй. Дай короткий ответ."),
				new UserChatMessage("Ответь на вопрос: Какой суд будет рассматривать иск к представителям из другой галактики?"),
			],
			new ChatCompletionOptions()
			{
				MaxOutputTokenCount = 2048,
			});

			string description = creativeWriterResult.Value.Content[0].Text;
			Console.WriteLine($"Creative helper's creature description:\n{description}");
		}

		#region

		private static string GetDateTimeNow()
		{
			return DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
		}

		private static string GetCurrentLocation()
		{
			return "Северное Тушино, Москва";
		}

		private static string GetCurrentWeather(string location, string unit = "celsius")
		{
			return $"31 {unit} в {location}";
		}

		#endregion

		#region

		private static readonly ChatTool getDateTimeNowTool = ChatTool.CreateFunctionTool(
			functionName: nameof(GetDateTimeNow),
			functionDescription: "Текущие дата и время"
		);

		private static readonly ChatTool getCurrentLocationTool = ChatTool.CreateFunctionTool(
			functionName: nameof(GetCurrentLocation),
			functionDescription: "Местоположение пользователя"
		);

		private static readonly ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
			functionName: nameof(GetCurrentWeather),
			functionDescription: "Погода в указанном месте",
			functionParameters: BinaryData.FromBytes(Utils.Resource.GetBytesResource("LLMConnectorLibrary.FunctionParameters.get_we.json"))
		);

		#endregion

		public static void Example03_FunctionCalling()
		{
			OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions
			{
				Endpoint = new Uri(""),
				NetworkTimeout = new TimeSpan(hours: 0, minutes: 10, seconds: 0),
			};

			OpenAIClient client = new(
				new ApiKeyCredential(DEFAULT_OPENAI_API_KEY),
				openAIClientOptions);

			ChatClient chatClient = client.GetChatClient(model: "LLMName");

			#region

			List<ChatMessage> messages = [.. new ChatMessage[]
			{
				new UserChatMessage("Какая сегодня дата? Сколько сейчас времени? Какая погода в данное время?"),
			}];

			ChatCompletionOptions options = new()
			{
				Tools = { getCurrentLocationTool, getCurrentWeatherTool, getDateTimeNowTool },
			};

			#endregion

			#region
			bool requiresAction;

			do
			{
				requiresAction = false;
				ChatCompletion completion = chatClient.CompleteChat(messages, options);

				switch (completion.FinishReason)
				{
					case ChatFinishReason.Stop:
						{
							messages.Add(new AssistantChatMessage(completion));
							break;
						}

					case ChatFinishReason.ToolCalls:
						{
							messages.Add(new AssistantChatMessage(completion));

							foreach (ChatToolCall toolCall in completion.ToolCalls)
							{
								switch (toolCall.FunctionName)
								{
									case nameof(GetDateTimeNow):
										{
											string toolResult = GetDateTimeNow();
											messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
											break;
										}
									case nameof(GetCurrentLocation):
										{
											string toolResult = GetCurrentLocation();
											messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
											break;
										}
									case nameof(GetCurrentWeather):
										{
											JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
											bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
											bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

											if (!hasLocation)
											{
												throw new ArgumentNullException(nameof(location), "The location argument is required.");
											}

											string toolResult = hasUnit
												? GetCurrentWeather(location.GetString(), unit.GetString())
												: GetCurrentWeather(location.GetString());
											messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
											break;
										}

									default:
										{
											throw new NotImplementedException();
										}
								}
							}
							requiresAction = true;
							break;
						}

					case ChatFinishReason.Length:
						throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

					case ChatFinishReason.ContentFilter:
						throw new NotImplementedException("Omitted content due to a content filter flag.");

					case ChatFinishReason.FunctionCall:
						throw new NotImplementedException("Deprecated in favor of tool calls.");

					default:
						throw new NotImplementedException(completion.FinishReason.ToString());
				}
			} while (requiresAction);

			#endregion

			#region

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"DateTime: {DateTime.Now}\n\n");
			stringBuilder.Append($"Model: {"LLMName"}\n\n");

			foreach (ChatMessage message in messages)
			{
				switch (message)
				{
					case UserChatMessage userMessage:
						stringBuilder.Append($"[USER]:");
						stringBuilder.Append($"{userMessage.Content[0].Text}\n");
						break;
					case AssistantChatMessage assistantMessage when assistantMessage.Content.Count > 0:
						stringBuilder.Append($"[ASSISTANT]:");
						stringBuilder.Append($"{assistantMessage.Content[0].Text}\n");
						break;
					case ToolChatMessage toolMessage when toolMessage.Content.Count > 0:
						stringBuilder.Append($"[TOOL]:");
						stringBuilder.Append($"{toolMessage.Content[0].Text}\n");
						break;
					default:
						stringBuilder.Append($"[other]\n");
						break;
				}
			}
			#endregion

			//Utils.Dialogs.ShowMessageDialog(stringBuilder.ToString());
		}
	}
}

#pragma warning restore OPENAI001 // Тип предназначен только для оценки и может быть изменен или удален в будущих обновлениях. Чтобы продолжить, скройте эту диагностику.
