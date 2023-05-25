using System.Runtime.Serialization;

namespace verse_interpreter.lib.Exceptions
{
    [Serializable]
    internal class NotEqualArityException : Exception
    {
        public NotEqualArityException()
        {
        }

        public NotEqualArityException(string? message) : base(message)
        {
        }

        public NotEqualArityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NotEqualArityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}