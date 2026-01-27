// Ignore Spelling: Utils Llm

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace LLMConnectorLibrary.Utils
{
	public static class Resource
	{
		public static string GetStringResource(string resourceName)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			StreamReader stream = new(stream: assembly.GetManifestResourceStream(name: resourceName));

			if (stream != null)
			{
				return stream.ReadToEnd();
			}
			else
			{
				throw new ArgumentException($"Ресурс '{resourceName}' не найден.");
			}
		}

		public static byte[] GetBytesResource(string resourceName)
		{
			return Encoding.UTF8.GetBytes(GetStringResource(resourceName: resourceName));
		}
	}
}
