// Ignore Spelling: uri OPENAI Llm

using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Models;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatFinishReason = OpenAI.Chat.ChatFinishReason;
using ChatMessage = OpenAI.Chat.ChatMessage;


namespace LLMConnectorLibrary
{
	public class LLMOpenAI
	{
		private const string OPENAI_API_KEY = "OPENAI_API_KEY";
		private const string RELATIVE_URI = "v1";

		private static readonly IDictionary<Uri, OpenAIClient> openAIClientCollection = new Dictionary<Uri, OpenAIClient>();
		private static TimeSpan _timeout;

		private static OpenAIClient GetOpenAIClient(Uri uri, TimeSpan timeout)
		{
			Uri baseUri = new(uri, RELATIVE_URI);

			if (!openAIClientCollection.ContainsKey(baseUri))
			{
				openAIClientCollection.Add(baseUri, CreateOpenAIClient(uri: baseUri, timeout: timeout));
			}
			else if (_timeout != timeout)
			{
				openAIClientCollection[baseUri] = CreateOpenAIClient(uri: baseUri, timeout: timeout);
			}
			return openAIClientCollection[baseUri];
		}

		private static OpenAIClient CreateOpenAIClient(Uri uri, TimeSpan timeout)
		{
			_timeout = timeout;

			OpenAIClientOptions options = new()
			{
				Endpoint = uri,
				NetworkTimeout = timeout,
			};
			return new(new ApiKeyCredential(OPENAI_API_KEY), options);
		}

		public static IEnumerable<string> GetModelsName(Uri uri, TimeSpan timeout)
		{
			return GetModels(uri: uri, timeout).Select(x => x.Id);
		}

		public static IEnumerable<OpenAIModel> GetModels(Uri uri, TimeSpan timeout)
		{
			try
			{
				var modelsResult = GetModelsCollection(uri, timeout);
				return [.. modelsResult];
			}
			catch (Exception)
			{
				return [];
			}
		}

		private static OpenAIModelCollection GetModelsCollection(Uri uri, TimeSpan timeout)
		{
			OpenAIModelClient modelClient = GetModelClient(uri, timeout);
			try
			{
				var result = modelClient.GetModels();
				return result.Value;
			}
			catch (Exception ex)
			{
				throw new Exception("Ошибка при подключении клиента OpenAI.", ex.InnerException);
			}
		}

		private async static Task<OpenAIModelCollection> GetModelsCollectionAsync(Uri uri, TimeSpan timeout)
		{
			OpenAIModelClient modelClient = GetModelClient(uri, timeout);
			try
			{
				var result = await modelClient.GetModelsAsync();
				return result.Value;
			}
			catch (Exception ex)
			{
				throw new Exception("Ошибка при подключении клиента OpenAI.", ex.InnerException);
			}
		}


		private static OpenAIModelClient GetModelClient(Uri uri, TimeSpan timeout)
		{
			bool isAvailable = Utils.Net.CheckHostByHttp(uri);

			if (isAvailable)
			{
				OpenAIClient client = GetOpenAIClient(uri, timeout);
				return client.GetOpenAIModelClient();
			}
			else
			{
				throw new Exception(string.Format("Хост по адресу [{0}] не доступен.", uri));
			}
		}

		public async static Task<string> SendMessageAsync(Uri uri, TimeSpan timeout, string model, ChatOptions options, string systemMessage, IEnumerable<string> userMessages)
		{
			OpenAIClient client = GetOpenAIClient(uri, timeout);
			ChatClient chatClient = client.GetChatClient(model: model);

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
				});
			return creativeWriterResult.Value.Content[0].Text;
		}

		public readonly struct ChatOptions
		{
			public static readonly ChatOptions Empty = new(4096, 0, 0, 0, 0);

			public static readonly ChatOptions Standard = new(4096, 0.3f, 0, 0.3f, 0.2f);

			private ChatOptions(int? maxOutputTokenCount,
				float? frequencyPenalty, 
				float? presencePenalty, 
				float? temperature, 
				float? topP)
			{
				MaxOutputTokenCount = maxOutputTokenCount;
				FrequencyPenalty = frequencyPenalty;
				PresencePenalty = presencePenalty;
				Temperature = temperature;
				TopP = topP;
			}

			/// <summary>
			/// Максимальное количество токенов в ответе.
			/// Значение по умолчанию 4096.
			/// </summary>
			public int? MaxOutputTokenCount { get; }

			/// <summary>
			/// Frequency penalty ограничивает токены в зависимости от того, как часто они встречаются в тексте на данный момент.
			/// Если вы присутствует чрезмерное использование одних и тех же слов в сгенерированном результате, возможно,
			/// следует увеличить значение этого параметра.
			/// Значения от -2 до 2. Значение по умолчанию: 0.
			/// </summary>
			public float? FrequencyPenalty { get; }

			/// <summary>
			/// Presence penalty ограничивает токены на основании того, появляются ли они в сгенерированном тексте до сих пор,
			/// независимо от того, как часто они встречаются.
			/// Значения от -2.0 до 2.0. Значение по умолчанию: 0.
			/// </summary>
			public float? PresencePenalty { get; }

			/// <summary>
			/// Temperature контролирует случайность и креативность генерируемого текста. Низкие значения делают модель более
			/// детерминированной и ориентированной на наиболее вероятные ответы. Это подходит для задач, требующих точности
			/// и согласованности, например, для ответов на фактические вопросы. Высокие значения вносят креативность и 
			/// разнообразие, позволяя модели исследовать менее вероятные варианты. Это полезно для творческого письма, 
			/// мозгового штурма, создания стихов.
			/// Диапазон температур обычно составляет от 0.0 до 2.0. Значение по умолчанию: 0.
			/// </summary>
			public float? Temperature { get; }

			/// <summary>
			/// Top-P (nucleus sampling) — метод сэмплирования, который управляет уровнем случайности и креативности при 
			/// выборе следующего токена в генерируемой последовательности. Высокое значение p (близкое к 1) включает больше токенов
			/// с меньшими вероятностями. Результат становится более случайным и разнообразным, но может иногда терять связность
			/// или релевантность. Низкое значение p(например, 0,5 или 0,7) включает меньше самых вероятных токенов. Результат
			/// более предсказуемый, сфокусированный, но может быть менее интересным и склонным к повторениям.
			/// Диапазон от 0.0 до 1.0. Значение по умолчанию: 0.
			/// </summary>
			public float? TopP { get; }
		}

		public async static Task<ReadOnlyMemory<float>> GetEmbeddingAsync(Uri uri, TimeSpan timeout, string model, string input)
		{
			OpenAIClient client = GetOpenAIClient(uri, timeout);

			IEmbeddingGenerator<string, Embedding<float>> generator = client
				.GetEmbeddingClient(model: model)
				.AsIEmbeddingGenerator();

			return await generator.GenerateVectorAsync(input);
		}

		public static IEnumerable<string> TestEmbedding(Uri uri, TimeSpan timeout, IEnumerable<string> models)
		{
			OpenAIClient client = GetOpenAIClient(uri, timeout);

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
				catch (Exception)
				{
				}
				finally
				{
					generator.Dispose();
				}
			}
			return result;
		}

		public delegate void ProgressChanged(int received, int totalToReceive, int progressPercentage);

		public async static Task<IEnumerable<(int key, string description, ReadOnlyMemory<float> vector)>> GetEmbeddingAsync(
			Uri uri,
			TimeSpan timeout,
			string model,
			IEnumerable<(int key, string description)> store,
			ProgressChanged progress)
		{
			OpenAIClient client = GetOpenAIClient(uri, timeout);


			IEmbeddingGenerator<string, Embedding<float>> generator = client
				.GetEmbeddingClient(model: model)
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
				new ApiKeyCredential(OPENAI_API_KEY),
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
				new ApiKeyCredential(OPENAI_API_KEY),
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
