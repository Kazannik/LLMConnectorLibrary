// Ignore Spelling: Utils

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace LLMConnectorLibrary.Utils
{
	internal static partial class Argument
	{
		public static void AssertNotNull<T>(T value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void AssertNotNull<T>(T? value, string name)
			where T : struct
		{
			if (!value.HasValue)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void AssertNotNullOrEmpty<T>(IEnumerable<T> value, string name)
		{
			if (value is null)
			{
				throw new ArgumentNullException(name);
			}
			if (value is ICollection<T> collectionOfT && collectionOfT.Count == 0)
			{
				throw new ArgumentException("Value cannot be an empty collection.", name);
			}
			if (value is ICollection collection && collection.Count == 0)
			{
				throw new ArgumentException("Value cannot be an empty collection.", name);
			}
			using (IEnumerator<T> e = value.GetEnumerator())
			{
				if (!e.MoveNext())
				{
					throw new ArgumentException("Value cannot be an empty collection.", name);
				}
			}
		}

		public static void AssertNotNullOrWhiteSpace(string value, string name)
		{
			if (value is null)
			{
				throw new ArgumentNullException(name);
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException("Value cannot be an empty string.", name);
			}
		}

		public static void AssertType<T>(T value, Type type)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			if (!type.IsAssignableFrom(value.GetType()))
			{
				throw new TypeAccessException(string.Format(CultureInfo.CurrentCulture,
					"Не удалось привести тип аргумента: {0} к поддерживаемому типу: {1}.", value.GetType().ToString(), typeof(T).ToString()));
			}
		}
	}
}
