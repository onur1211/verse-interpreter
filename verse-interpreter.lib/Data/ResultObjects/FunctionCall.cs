using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.ParseVisitors
{
    public class FunctionCall
    {
        public Function Function { get; set; }

        public FunctionParameters Parameters { get; set; }

        public FunctionCall(FunctionParameters parameters, Function function)
        {
            Parameters = parameters;
            Function = function;
        }
    }
}