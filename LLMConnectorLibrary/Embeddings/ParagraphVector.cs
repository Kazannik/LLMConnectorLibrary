using Microsoft.Extensions.VectorData;
using System;

namespace LLMConnectorLibrary.Embeddings
{
	internal class ParagraphVector(int key, string description)
	{
		[VectorStoreKey]
		public int Key { get; } = key;

		[VectorStoreData]
		public string Description { get; } = description;

		[VectorStoreVector(
			Dimensions: 384,
			DistanceFunction = DistanceFunction.CosineSimilarity)]
		public ReadOnlyMemory<float> Vector { get; set; }
	}
}
