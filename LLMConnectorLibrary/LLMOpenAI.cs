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

		public static IEnumerable<string> GetModelsName(Uri uri)
		{
			return GetModels(uri: uri).Select(x => x.Id);
		}

		public static IEnumerable<OpenAIModel> GetModels(Uri uri)
		{
			try
			{
				var modelsResult = GetModelsCollection(uri);
				return [.. modelsResult];
			}
			catch (Exception)
			{
				return [];
			}
		}

		private static OpenAIModelCollection GetModelsCollection(Uri uri)
		{
			OpenAIModelClient modelClient = GetModelClient(uri);
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

		private async static Task<OpenAIModelCollection> GetModelsCollectionAsync(Uri uri)
		{
			OpenAIModelClient modelClient = GetModelClient(uri);
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


		private static OpenAIModelClient GetModelClient(Uri uri)
		{
			OpenAIClientOptions options = new()
			{
				Endpoint = uri
			};

			bool isAvailable = Utils.Net.CheckHostByHttpAsync(
				new Uri(baseUri: options.Endpoint, relativeUri: options.Endpoint.AbsolutePath + "/models"))
				.GetAwaiter().GetResult();

			if (isAvailable)
			{
				OpenAIClient client = new(
					new ApiKeyCredential(OPENAI_API_KEY),
					options);

				return client.GetOpenAIModelClient();
			}
			else
			{
				throw new Exception(string.Format("Хост по адресу [{0}] не доступен.", uri));
			}
		}

		public async static Task<string> SendMessageAsync(Uri uri, string model, string systemMessage, IEnumerable<string> userMessages)
		{
			OpenAIClientOptions options = new()
			{
				Endpoint = uri,
				NetworkTimeout = new TimeSpan(hours: 0, minutes: 10, seconds: 0),
			};

			OpenAIClient client = new(
				new ApiKeyCredential(OPENAI_API_KEY),
				options);

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
					MaxOutputTokenCount = 2048,
					Temperature = 0,
				});
			return creativeWriterResult.Value.Content[0].Text;
		}

		public async static Task<ReadOnlyMemory<float>> GetEmbeddingAsync(Uri uri, string model, string input)
		{
			OpenAIClientOptions options = new()
			{
				Endpoint = uri,
				NetworkTimeout = new TimeSpan(hours: 0, minutes: 10, seconds: 0),
			};

			IEmbeddingGenerator<string, Embedding<float>> generator =
				new OpenAIClient(new ApiKeyCredential(OPENAI_API_KEY), options)
				.GetEmbeddingClient(model: model)
				.AsIEmbeddingGenerator();

			return await generator.GenerateVectorAsync(input);
		}

		public static void CreateMultipleClients()
		{
			OpenAIClientOptions options = new OpenAIClientOptions
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

			OpenAIClient client = new OpenAIClient(
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
