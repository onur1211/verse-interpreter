using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib.EventArguments
{
    public class ArithmeticExpressionResolvedEventArgs : EventArgs
    {
        public ArithmeticExpressionResolvedEventArgs(ArithmeticExpression result)
        {
            Result = result;
        }

        public ArithmeticExpression Result { get; }
    }
}