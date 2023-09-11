namespace verse_interpreter.lib.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException()
        {
        }

        public InvalidTypeException(string typeName)
            : base($"The type: '{typeName}' is invalid.")
        {
        }

        public InvalidTypeException(string typeName, Exception innerException)
            : base($"The type: '{typeName}' is invalid.", innerException)
        {
        }
    }
}
