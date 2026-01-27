using System;
using System.Linq;

namespace LLMConnectorLibrary.Embeddings
{
	internal static class Average
	{
		public static ReadOnlyMemory<float> GetAverageVector(ReadOnlyMemory<float>[] vectors)
		{


			foreach (ReadOnlyMemory<float> vector in vectors)
			{



			}

			return new ReadOnlyMemory<float>();
		}


		public static void Main()
		{
			float[] data = { 10, 20, 30, 40, 50 };



			float min = data.Min();
			float max = data.Max();

			// 1. Нормализуем каждое число: (x - min) / (max - min)
			// 2. Вычисляем среднее арифметическое нормализованных значений
			float normalizedAverage = data
				.Select(x => (x - min) / (max - min))
				.Average();

			Console.WriteLine($"Нормализованное среднее: {normalizedAverage}");
			// Для данных выше это будет 0.5
		}

	}
}
