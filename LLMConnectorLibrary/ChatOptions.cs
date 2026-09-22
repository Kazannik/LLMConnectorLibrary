namespace LLMConnectorLibrary
{
	public readonly struct ChatOptions : IChatOptions
	{
		/// <summary>
		/// Базовые настройки.(Temperature: 0.0, Top-P: 0.0, Frequency Penalty: 0.0, Presence Penalty: 0.0).
		/// </summary>
		public static readonly IChatOptions Basic =
			new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0,
				presencePenalty: 0,
				temperature: 0.0f,
				topP: 0.0f,
				name: "Базовые настройки");

		/// <summary>
		/// Кодинг и математика.(Temperature: 0.0, Top-P: 1.0, Frequency Penalty: 0.0, Presence Penalty: 0.0).
		/// </summary>
		public static readonly IChatOptions CodingAndMathematics = 
			new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0,
				presencePenalty: 0,
				temperature: 0.0f,
				topP: 1.0f,
				name: "Кодинг и математика");

		/// <summary>
		/// Аналитика и факты.(Temperature: 0.2, Top-P: 1.0, Frequency Penalty: 0.0, Presence Penalty: 0.0).
		/// </summary>
		public static readonly IChatOptions AnalyticsAndFacts = 
			new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0,
				presencePenalty: 0,
				temperature: 0.2f,
				topP: 1.0f,
				name: "Аналитика и факты");

		/// <summary>
		/// Бизнес-переписка.(Temperature: 1.0, Top-P: 0.4, Frequency Penalty: 0.1, Presence Penalty: 0.1).
		/// </summary>
		public static readonly IChatOptions BusinessCorrespondence = 
			new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0.1f,
				presencePenalty: 0.1f,
				temperature: 1.0f,
				topP: 0.4f,
				name: "Бизнес-переписка");

		/// <summary>
		/// Стандартный чат.(Temperature: 0.7, Top-P: 1.0, Frequency Penalty: 0.3, Presence Penalty: 0.3).
		/// </summary>
		public static readonly IChatOptions StandardChat = 
			new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0.3f,
				presencePenalty: 0.3f,
				temperature: 0.7f,
				topP: 1.0f,
				name: "Стандартный чат");

		/// <summary>
		/// Копирайтинг и блоги. (Temperature: 1.0, Top-P: 0.9, Frequency Penalty: 0.5, Presence Penalty: 0.4).
		/// </summary>
		public static readonly IChatOptions CopywritingAndBlogging = 
			new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0.5f,
				presencePenalty: 0.4f,
				temperature: 1.0f,
				topP: 0.9f,
				name: "Копирайтинг и блоги");

		/// <summary>
		/// Мозговой штурм. (Temperature: 1.2 – 1.5, Top-P: 1.0, Frequency Penalty: 0.6, Presence Penalty: 0.8).
		/// </summary>
		public static readonly IChatOptions Brainstorming = 
			new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0.6f,
				presencePenalty: 0.8f,
				temperature: 1.3f,
				topP: 1.0f,
				name: "Мозговой штурм");

		public static IChatOptions GetBrainstorming(float temperature)
		{
			return new ChatOptions(
				maxOutputTokenCount: 4096,
				frequencyPenalty: 0.6f,
				presencePenalty: 0.8f,
				temperature: temperature,
				topP: 1.0f,
				name: "Мозговой штурм");
		}

		public static readonly IChatOptions Empty = new ChatOptions(4096, 0, 0, 0, 0);

		public static IChatOptions Create(
			int maxOutputTokenCount,
			float frequencyPenalty,
			float presencePenalty,
			float temperature,
			float topP,
			string name = "") => new ChatOptions(maxOutputTokenCount: maxOutputTokenCount, frequencyPenalty: frequencyPenalty, presencePenalty: presencePenalty, temperature: temperature, topP: topP, name: name);

		private ChatOptions(
			int? maxOutputTokenCount,
			float? frequencyPenalty,
			float? presencePenalty,
			float? temperature,
			float? topP,
			string name = "")
		{
			MaxOutputTokenCount = maxOutputTokenCount;
			FrequencyPenalty = frequencyPenalty;
			PresencePenalty = presencePenalty;
			Temperature = temperature;
			TopP = topP;
			Name = name;
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

		public string Name { get; }

		public override string ToString()
		{
			return Name;
		}
	}
}
