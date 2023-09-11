namespace verse_interpreter.lib.Lookup;

public class UnknownFunctionException : Exception
{
	public UnknownFunctionException(string? message) : base(message)
	{
	}
}