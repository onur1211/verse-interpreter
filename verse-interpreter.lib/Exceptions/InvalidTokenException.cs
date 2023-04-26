using System.Runtime.Serialization;

namespace verse_interpreter.lib.Lexer
{
    internal class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string? message) : base(message)
        {
        }

        public InvalidTokenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}