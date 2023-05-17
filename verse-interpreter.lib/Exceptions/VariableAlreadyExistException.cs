using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
