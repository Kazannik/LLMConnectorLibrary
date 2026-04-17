using LLMConnectorLibrary.Embeddings;

namespace LLMConnectorLibrary.Strategy
{
	internal class TextEntity(long fileId, string text, int start, int end)
	{
		public long FileId { get; } = fileId;

		public string Text { get; } = text;

		public int Start { get; } = start;

		public int End { get; } = end;

		public ParagraphVector? Vector { get; set; }
	}
}
