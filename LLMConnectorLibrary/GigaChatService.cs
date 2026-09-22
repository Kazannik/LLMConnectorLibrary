// Ignore Spelling: uri OPENAI Llm

using GigaChat;
using GigaChat.AccessTokens;
using GigaChat.Chat;
using GigaChat.Models;
using LLMConnectorLibrary.Authentication;
using LLMConnectorLibrary.Models;
using Microsoft.Extensions.AI;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ChatMessage = GigaChat.Chat.ChatMessage;

namespace LLMConnectorLibrary
{
	public static class GigaChatService
	{
		private const string DEFAULT_GIGACHAT_API_KEY = "GIGACHAT_API_KEY";

		public static GigaChatClient CreateGigaChatClient(IClientOptions options, string key = DEFAULT_GIGACHAT_API_KEY)
		{
			GigaChatClientOptions clientOptions = new()
			{
				Endpoint = options.Endpoint,
				NetworkTimeout = options.Timeout,
			};
			return new(new ApiKeyCredential(key), clientOptions);
		}

		public static GigaChatClient CreateGigaChatClient(IClientOptions options, X509Certificate2 certificate, string key = DEFAULT_GIGACHAT_API_KEY)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

			HttpClientHandler handler = NetService.CreateHttpClientHandler(certificate: certificate);
			
			HttpClient httpClient = new(handler);

			GigaChatClientOptions clientOptions = new()
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
				IPreAuthenticationProfile<GigaChatClient> authenticationProfile = (IPreAuthenticationProfile<GigaChatClient>)profile;
				if (!authenticationProfile.AccessTokenIsValid)
				{
					GigaChatAccessTokenClient tokenClient = authenticationProfile.OauthBaseClient.GetGigaChatAccessTokenClient();
					var accessToken = tokenClient.GetAccessToken(Guid.NewGuid().ToString());
					authenticationProfile.SetAccessToken(accessToken.Value.AccessToken);
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
				GigaChatModelCollection modelsResult = GetModelsCollection(profile: profile);
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
				GigaChatModelCollection modelsResult = await GetModelsCollectionAsync(profile: profile);
				return ModelsCollection.Create(profile, modelsResult);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private static GigaChatModelCollection GetModelsCollection(IAuthenticationProfile profile)
		{
			GigaChatModelClient modelClient = GetModelClient(profile: profile);
			try
			{
				ClientResult<GigaChatModelCollection> result = modelClient.GetModels();
				return result.Value;
			}
			catch (HttpRequestException ex)
			{
				throw new ApplicationException($"Ошибка при отправке HTTP запроса: {ex.Message}", ex);
			}
			catch (ClientResultException ex)
			{
				throw new ApplicationException($"Ошибка при отправке HTTP запроса: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Неизвестная ошибка: {ex.Message}", ex);
			}
		}

		private async static Task<GigaChatModelCollection> GetModelsCollectionAsync(IAuthenticationProfile profile)
		{
			GigaChatModelClient modelClient = GetModelClient(profile: profile);
			try
			{
				var result = await modelClient.GetModelsAsync();
				return result.Value;
			}
			catch (HttpRequestException ex)
			{
				throw new ApplicationException($"Ошибка при отправке HTTP запроса: {ex.Message}", ex);
			}
			catch (ClientResultException ex)
			{
				throw new ApplicationException($"Ошибка при отправке HTTP запроса: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Неизвестная ошибка: {ex.Message}", ex);
			}
		}

		private static GigaChatModelClient GetModelClient(IAuthenticationProfile profile)
		{
			AccessTokenValid(profile: profile);

			GigaChatClient client = (GigaChatClient)profile.LLMBaseClient;
			return client.GetGigaChatModelClient();			
		}

		public async static Task<string> SendMessageAsync(IModel model, IChatOptions options, string systemMessage, IEnumerable<string> userMessages)
		{
			AccessTokenValid(profile: model.Profile);

			GigaChatClient client = (GigaChatClient)model.Profile.LLMBaseClient;

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
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
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

		public async static Task<ReadOnlyMemory<float>> GetEmbeddingAsync(IModel model, string input)
		{
			AccessTokenValid(profile: model.Profile);

			GigaChatClient client = (GigaChatClient)model.Profile.LLMBaseClient;

			IEmbeddingGenerator<string, Embedding<float>> generator = client
				.GetEmbeddingClient(model: model.Id)
				.AsIEmbeddingGenerator();

			return await generator.GenerateVectorAsync(input);
		}

		public static IEnumerable<string> TestEmbedding(IAuthenticationProfile profile, IEnumerable<string> models)
		{
			AccessTokenValid(profile: profile);

			GigaChatClient client = (GigaChatClient)profile.LLMBaseClient;

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

			GigaChatClient client = (GigaChatClient)model.Profile.LLMBaseClient;
		

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

		//public static void CreateMultipleClients()
		//{
		//	GigaChatClientOptions options = new()
		//	{
		//		Endpoint = new Uri("")
		//	};

		//	GigaChatClient client = new(
		//		new ApiKeyCredential(DEFAULT_OPENAI_API_KEY),
		//		options);

		//	GigaChatModelClient modelClient = client.GetGigaChatModelClient();
		//	modelClient.GetModels();

		//	EmbeddingClient embeddingClient = client.GetEmbeddingClient("mistral");

		//	ChatClient chatClient = client.GetChatClient("mistral");

		//	ClientResult<ChatCompletion> creativeWriterResult = chatClient.CompleteChat(
		//	[
		//		new SystemChatMessage("Ты профессиональный юрист. Проводишь консультацию клиента. Не фантазируй. Дай короткий ответ."),
		//		new UserChatMessage("Ответь на вопрос: Какой суд будет рассматривать иск к представителям из другой галактики?"),
		//	],
		//	new ChatCompletionOptions()
		//	{
		//		MaxOutputTokenCount = 2048,
		//	});

		//	string description = creativeWriterResult.Value.Content[0].Text;
		//	Console.WriteLine($"Creative helper's creature description:\n{description}");
		//}

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
	}
}

#pragma warning restore OPENAI001 // Тип предназначен только для оценки и может быть изменен или удален в будущих обновлениях. Чтобы продолжить, скройте эту диагностику.
