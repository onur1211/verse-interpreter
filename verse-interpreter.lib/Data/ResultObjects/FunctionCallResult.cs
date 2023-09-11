using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib.Data.ResultObjects
{
    public class FunctionCallResult
    {
        public ArithmeticExpression? ArithmeticExpression { get; set; }

        public StringExpression? StringExpression { get; set; }

        public ForExpression? ForExpression { get; set; }
        public bool WasValueResolved { get; set; }
        public bool IsVoid { get; set; }
        public Variable? Variable { get; internal set; }
    }
}
