using System.Collections.Generic;

namespace LLMConnectorLibrary.Strategy
{
	internal class Converter(string llmName) : ChartEntity(llmName)
	{
		public IEnumerable<TextEntity>? TextEntities { get; set; }

		public string GetResult()
		{
			return string.Empty;
		}
	}
}
