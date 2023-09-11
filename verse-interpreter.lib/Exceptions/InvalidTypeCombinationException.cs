using System.Runtime.Serialization;

namespace verse_interpreter.lib.Exceptions
{
	[Serializable]
	internal class InvalidTypeCombinationException : Exception
	{
		public InvalidTypeCombinationException()
		{
		}

		public InvalidTypeCombinationException(string? message) : base(message)
		{
		}

		public InvalidTypeCombinationException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected InvalidTypeCombinationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}