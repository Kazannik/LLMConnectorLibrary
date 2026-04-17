using System;

namespace LLMConnectorLibrary.Strategy
{
	abstract class ChartEntity(string llmName)
	{
		public string LLMName { get; protected set; } = llmName;

		public TimeSpan? Timeout { get; protected set; }

		public string? SystemMessage { get; protected set; }

		public float? Temperature { get; protected set; }

		public int? MaxOutputTokenCount { get; protected set; }
	}
}
