namespace verse_interpreter.lib.Exceptions
{
	public class VariableDoesNotExistException : Exception
	{
		public VariableDoesNotExistException()
		{
		}

		public VariableDoesNotExistException(string variableName)
			: base($"Variable '{variableName}' does not exist.")
		{
		}

		public VariableDoesNotExistException(string variableName, Exception innerException)
			: base($"Variable '{variableName}' does not exist.", innerException)
		{
		}
	}
}
