using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.ParseVisitors
{
    public class FunctionCallItem
    {
        public Function Function { get; set; }

        public FunctionParameters Parameters { get; set; }

        public FunctionCallItem(FunctionParameters parameters, Function function)
        {
            Parameters = parameters;
            Function = function;
        }
    }
}