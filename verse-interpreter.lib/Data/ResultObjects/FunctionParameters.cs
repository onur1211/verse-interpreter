using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data.ResultObjects
{
    public class FunctionParameters
    {
        public FunctionParameters()
        {
            Parameters = new List<Variable>();
        }
        public List<Variable> Parameters { get; set; } = null!;

        public int ParameterCount { get {  return Parameters.Count; } }
    }
}