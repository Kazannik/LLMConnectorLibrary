using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace LLMConnectorLibrary.Embeddings
{
	internal class Class1
	{
		private const string OPENAI_API_KEY = "OPENAI_API_KEY";

		public async Task LoadAsync()
		{
			Uri uri = new(baseUri: new Uri("http://localhost:11434"), relativeUri: "/v1");
			string model = "";

			OpenAIClientOptions options = new()
			{
				Endpoint = uri
			};


			List<ParagraphVector> paragraphVectors = [];
			//[
			//	new() {
			//			Key = 0,
			//			DocumentName = "Azure App Service",
			//			Description = "Host .NET, Java, Node.js, and Python web applications and APIs in a fully managed Azure service. You only need to deploy your code to Azure. Azure takes care of all the infrastructure management like high availability, load balancing, and autoscaling."
			//	},
			//	new() {
			//			Key = 1,
			//			DocumentName = "Azure Service Bus",
			//			Description = "A fully managed enterprise message broker supporting both point to point and publish-subscribe integrations. It's ideal for building decoupled applications, queue-based load leveling, or facilitating communication between microservices."
			//	},
			//	new() {
			//			Key = 2,
			//			DocumentName = "Azure Blob Storage",
			//			Description = "Azure Blob Storage allows your applications to store and retrieve files in the cloud. Azure Storage is highly scalable to store massive amounts of data and data is stored redundantly to ensure high availability."
			//	},
			//	new() {
			//			Key = 3,
			//			DocumentName = "Microsoft Entra ID",
			//			Description = "Manage user identities and control access to your apps, data, and resources."
			//	},
			//	new() {
			//			Key = 4,
			//			DocumentName = "Azure Key Vault",
			//			Description = "Store and access application secrets like connection strings and API keys in an encrypted vault with restricted access to make sure your secrets and your application aren't compromised."
			//	},
			//	new() {
			//			Key = 5,
			//			DocumentName = "Azure AI Search",
			//			Description = "Information retrieval at scale for traditional and conversational search applications, with security and options for AI enrichment and vectorization."
			//	}
			//];



			// Create the embedding generator.
			IEmbeddingGenerator<string, Embedding<float>> generator =
				new OpenAIClient(new ApiKeyCredential(OPENAI_API_KEY), options)
				.GetEmbeddingClient(model: model)
				.AsIEmbeddingGenerator();


			// Create and populate the vector store.
			var vectorStore = new InMemoryVectorStore();

			VectorStoreCollection<int, ParagraphVector> paragraphVectorsStore =
				vectorStore.GetCollection<int, ParagraphVector>("ParagraphsStore");

			await paragraphVectorsStore.EnsureCollectionExistsAsync();

			foreach (ParagraphVector paragraph in paragraphVectors)
			{
				paragraph.Vector = await generator.GenerateVectorAsync(paragraph.Description);
				await paragraphVectorsStore.UpsertAsync(paragraph);
			}


			// Convert a search query to a vector
			// and search the vector store.
			string query = "Which Azure service should I use to store my Word documents?";
			ReadOnlyMemory<float> queryEmbedding = await generator.GenerateVectorAsync(query);

			// Create the vector search options and set the filter
			var vectorSearchOptions = new VectorSearchOptions<ParagraphVector>
			{
				//Filter = r => r.Category == "External Definitions" && r.Tags.Contains("memory"), // Use a LINQ expression for filtering
			};

			IAsyncEnumerable<VectorSearchResult<ParagraphVector>> results =
				paragraphVectorsStore.SearchAsync(queryEmbedding, top: 1, options: vectorSearchOptions);

			await foreach (VectorSearchResult<ParagraphVector> result in results)
			{
				//Console.WriteLine($"Name: {result.Record.DocumentName}");
				Console.WriteLine($"Description: {result.Record.Description}");
				Console.WriteLine($"Vector match score: {result.Score}");
			}
		}
	}
}
