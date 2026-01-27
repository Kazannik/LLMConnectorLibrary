using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace LLMConnectorLibrary
{
	public class FunctionTool
	{
		public ChatTool GetChatTool()
		{
			BinaryData parameters = null;

			if (Parameters != null)
			{
				parameters = SerializePropertiesToBinaryData(Parameters, Strict);
			}


			bool? strict = Strict ? true : default;

			return ChatTool.CreateFunctionTool(
			functionName: Name,
			functionDescription: Description,
			functionParameters: parameters,
			functionSchemaIsStrict: strict
			);
		}

		/// <summary>
		/// Тип функции. Всегда должен быть  "function".
		/// </summary>
		public string Type => "function";

		/// <summary>
		/// Имя функции.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Подробная информация о том, когда и как использовать эту функцию
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Входные аргументы функции.
		/// </summary>
		public Parameter Parameters { get; }

		/// <summary>
		/// Следует ли применять строгий режим при вызове функции. При применении строгого режима AdditionalProperties = false, а все поля должны быть обязательными (включены в Required).
		/// </summary>
		public bool Strict { get; }

		public class Parameter
		{
			public string Type => "object";

			public IEnumerable<Property> Properties { get; }

			/// <summary>
			/// Возможны ли дополнительные свойства. По умолчанию значение true. Значение false указывает, что дополнительные свойства запрещены.
			/// </summary>
			public bool AdditionalProperties { get; }

			/// <summary>
			/// Обязательные свойства.
			/// </summary>
			public IEnumerable<string> Required { get; }

			public class Property
			{
				/// <summary>
				/// Наименование параметра.
				/// </summary>
				public string Name { get; }

				/// <summary>
				/// Тип.
				/// </summary>
				public TypeEnum Type { get; }

				/// <summary>
				/// Перечисление Enum
				/// </summary>
				public IEnumerable<string> Enum { get; }

				/// <summary>
				/// Подробная информация о свойстве
				/// </summary>
				public string Description { get; }

				/// <summary>
				/// Тип данных.
				/// </summary>
				public enum TypeEnum
				{
					@string,
					number,
					integer,
					@object,
					array,
					boolean,
					@null
				}
			}
		}

		private static string GetIndent(int count = 0)
		{
			return new string('\x0020', count);
		}

		private static BinaryData SerializePropertiesToBinaryData(Parameter parameter, bool strict, int indent = 0)
		{
			return BinaryData.FromBytes(Encoding.UTF8.GetBytes(SerializeProperties(parameter: parameter, strict: strict, indent: indent)));
		}

		private static string SerializeProperties(Parameter parameter, bool strict, int indent = 0)
		{
			StringBuilder parameterBuilder = new StringBuilder();
			parameterBuilder.Append(GetIndent(indent) + "{\n");
			parameterBuilder.Append(GetIndent(indent + 2) + "\"type\":\"object\",\n");
			parameterBuilder.Append(GetIndent(indent + 2) + "\"properties\":\"{\n");

			List<string> properties = new List<string>();

			foreach (Parameter.Property item in parameter.Properties)
			{
				StringBuilder propertyBuilder = new StringBuilder();
				propertyBuilder.Append(GetIndent(indent + 4) + "\"" + item.Name + "\": {\n");
				propertyBuilder.Append(GetIndent(indent + 6) + "\"type\": \"" + item.Type + "\",\n");
				propertyBuilder.Append(GetIndent(indent + 6) + "\"enum\": [" + string.Join(", ", item.Enum
					.Select(x => string.Format("\"{0}\"", x))) + "]\n");
				propertyBuilder.Append(GetIndent(indent + 6) + "\"description\": \"" + item.Description + "\"\n");
				propertyBuilder.Append(GetIndent(indent + 4) + "}");
				properties.Add(propertyBuilder.ToString());
			}
			parameterBuilder.Append(string.Join(",\n", properties));
			parameterBuilder.Append(GetIndent(indent + 2) + "},\n");
			parameterBuilder.Append(GetIndent(indent + 2) + "\"required\": [ " + string.Join(" ,", parameter.Required) + " ]");
			parameterBuilder.Append(GetIndent(indent) + "}\n");
			return parameterBuilder.ToString();
		}



		#region




		private static int GetLocationId(string locationName)
		{
			if (locationName.ToLower().Contains("москв"))
				return 100;
			else
				return 200;
		}

		private static int GetRowId(string rowName)
		{
			if (rowName.ToLower().Contains("зарегистр"))
				return 10;
			else
				return 20;
		}

		private static int GetColumnId(string columnName)
		{
			if (columnName.ToLower().Contains("всего"))
				return 1;
			else
				return 2;
		}

		private static string GetValue(int locationId, string period, int rowId, int columnId)
		{
			return $"в {period} зарегистрировано " + (locationId + rowId + columnId).ToString();
		}

		#endregion

		#region

		private static readonly ChatTool getLocationIdTool = ChatTool.CreateFunctionTool(
			functionName: nameof(GetLocationId),
			functionDescription: "Получить индекс места, территории, города, округа, района и т.д. по его наименованию (например, Красноярск, Омск и т.д.), который необходим для вызова функции GetValueе()."
		);

		private static readonly ChatTool getRowIdTool = ChatTool.CreateFunctionTool(
			functionName: nameof(GetRowId),
			functionDescription: "Получить индекс для категории учета преступлений (зарегистрировано преступлений, возбуждено уголовных дел и т.д.), который необходим для вызова функции GetValueе()"
		);

		private static readonly ChatTool getColumnIdTool = ChatTool.CreateFunctionTool(
			functionName: nameof(GetColumnId),
			functionDescription: "Получить индекс для категории преступлений (всего преступлений, коррупционных преступлений, экономических преступлений и т.д.), который необходим для вызова функции GetValueе()"
		);

		private static readonly ChatTool getValueTool = ChatTool.CreateFunctionTool(
			functionName: nameof(GetValue),
			functionDescription: "Количество зарегистрированных преступлений за определенный период на определенной территории.",
			functionParameters: BinaryData.FromBytes(Utils.Resource.GetBytesResource("WordHiddenPowers.Repository.Data.get_value.json"))
		);

		#endregion

		public static void Example03_FunctionCalling(ChatClient chatClient)
		{
			#region

			List<ChatMessage> messages = new List<ChatMessage>(new ChatMessage[]
			{
				new UserChatMessage("Какое количество преступлений зарегистрировано в Москве в 2025 году?"),
			});

			ChatCompletionOptions options = new ChatCompletionOptions()
			{
				Tools = { getLocationIdTool, getRowIdTool, getColumnIdTool, getValueTool },
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
									case nameof(GetLocationId):
										{
											JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
											bool hasLocationName = argumentsJson.RootElement.TryGetProperty("locationName", out JsonElement locationName);

											int toolResult = GetLocationId(locationName.ToString());
											messages.Add(new ToolChatMessage(toolCall.Id, toolResult.ToString()));
											break;
										}
									case nameof(GetRowId):
										{
											JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
											bool hasRowName = argumentsJson.RootElement.TryGetProperty("rowName", out JsonElement rowName);

											int toolResult = GetRowId(rowName.ToString());
											messages.Add(new ToolChatMessage(toolCall.Id, toolResult.ToString()));
											break;
										}
									case nameof(GetColumnId):
										{
											JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
											bool hasColumnName = argumentsJson.RootElement.TryGetProperty("columnName", out JsonElement columnName);

											int toolResult = GetColumnId(columnName.ToString());
											messages.Add(new ToolChatMessage(toolCall.Id, toolResult.ToString()));
											break;
										}
									case nameof(GetValue):
										{
											JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
											bool hasLocationId = argumentsJson.RootElement.TryGetProperty("locationId", out JsonElement locationId);
											///bool hasPeriod = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);
											bool hasRowId = argumentsJson.RootElement.TryGetProperty("rowId", out JsonElement rowId);
											bool hasColumnId = argumentsJson.RootElement.TryGetProperty("columnId", out JsonElement columnId);

											//if (!hasLocation)
											//{
											//	throw new ArgumentNullException(nameof(location), "The location argument is required.");
											//}

											string toolResult = GetValue(locationId.GetInt16(), DateTime.Now.ToString(), rowId.GetInt16(), columnId.GetInt16());

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
			stringBuilder.Append($"DateTime: {DateTime.Now.ToString()}\n\n");
			//stringBuilder.Append($"Model: {Services.OpenAIService.LLMName}\n\n");

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

			///Utils.Dialogs.ShowMessageDialog(stringBuilder.ToString());

		}

	}
}
