namespace verse_interpreter.lib.Exceptions
{
    public class VariableAlreadyExistException : Exception
    {
        public VariableAlreadyExistException()
        {
        }

        public VariableAlreadyExistException(string variableName)
            : base($"Variable '{variableName}' already exists.")
        {
        }

        public VariableAlreadyExistException(string variableName, Exception innerException)
            : base($"Variable '{variableName}' already exists.", innerException)
        {
        }
    }
}
