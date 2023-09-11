namespace verse_interpreter.lib.Exceptions
{
	public class UnknownTypeException : Exception
	{
		public UnknownTypeException()
		{
		}

		public UnknownTypeException(string typeName)
			: base($"The type: '{typeName}' is unknown.")
		{
		}

		public UnknownTypeException(string typeName, Exception innerException)
			: base($"The type: '{typeName}' is unknown.", innerException)
		{
		}
	}
}
