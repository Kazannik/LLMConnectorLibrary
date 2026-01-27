using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Uri uri  = new Uri(baseUri: new Uri("http://localhost:11434"), relativeUri: "/v1");
			try
			{
				IEnumerable<string> names = LLMConnectorLibrary.LLMOpenAI.GetModelsName(uri);
				foreach (string name in names)
				{
					Console.WriteLine(name);
				}				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException.Message);
			}
			finally
			{
				Console.ReadKey();
			}	
		}
	}
}
