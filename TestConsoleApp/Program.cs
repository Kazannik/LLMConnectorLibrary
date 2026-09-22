using LLMConnectorLibrary.GigaChat;
using LLMConnectorLibrary.GigaChat.Interfaces;
using LLMConnectorLibrary.GigaChat.Models;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static LLMConnectorLibrary.OpenAIService;
using static System.Net.Mime.MediaTypeNames;

namespace TestConsoleApp
{
	internal class Program
	{
		private static readonly X509Certificate2 certificate = new X509Certificate2(fileName: "gigaserver.pfx", password: "000000");
		
		//private static readonly IHttpService httpService = new HttpService(certificate: certificate);

		//static IHttpService httpService = new HttpService(ignoreTLS: false);
		//static ITokenService tokenService = new TokenService(httpService, "MDE5ZGI1ODEtMjZmMi03ZWYzLTllOWUtNzkzNjdlNjE5MGNkOmIyMWY4MmQ1LWVmNGYtNDM4Mi05NzM2LTk4MzU3MzU0OTFhNQ==", isCommercial: false);
		//static IGigaChat Chat = new GigaChat(tokenService, httpService, saveImage: false);

		//private static readonly IGigaChat Chat = new GigaChat(httpService, saveImage: false, baseUrl: "https://10.32.1.122:8080/");



		static void Main(string[] args)
		{

			

			//Uri uri = new Uri("http://localhost:11434");
			Uri uri = new Uri("https://10.32.1.122:8080");
			try
			{
				Console.WriteLine("READ MODELS...");
				
				//GigaChatModelCollection models = Chat.GetModelsAsync().GetAwaiter().GetResult();
				//foreach (GigaChatModel model in models.Data)
				//{
				//	Console.WriteLine(model.Id);
				//}

				//IEnumerable<string> names = LLMConnectorLibrary.OpenAIService.GetModelsName(uri, certificate, new TimeSpan(0, 3, 0));
				//foreach (string name in names)
				//{
				//	Console.WriteLine(name);
				//}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Console.ReadKey();
			}
		}
	}
}
