using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
