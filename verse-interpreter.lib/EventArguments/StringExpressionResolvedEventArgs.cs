using verse_interpreter.lib.Data.Expressions;

namespace verse_interpreter.lib.EventArguments
{
    public class StringExpressionResolvedEventArgs
    {
        public StringExpressionResolvedEventArgs(StringExpression result)
        {
            Result = result;
        }

        public StringExpression Result { get; }
    }
}