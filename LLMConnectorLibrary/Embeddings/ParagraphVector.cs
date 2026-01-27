using Microsoft.Extensions.VectorData;
using System;


namespace LLMConnectorLibrary.Embeddings
{
	internal class ParagraphVector
	{
		[VectorStoreKey]
		public int Key { get; set; }

		[VectorStoreData]
		public string DocumentName { get; set; }

		[VectorStoreData]
		public string Description { get; set; }

		[VectorStoreVector(
			Dimensions: 384,
			DistanceFunction = DistanceFunction.CosineSimilarity)]
		public ReadOnlyMemory<float> Vector { get; set; }

	}
}
