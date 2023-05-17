using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.EventArguments
{
    public class ExpressionTerminalVisited : EventArgs
    {
        public ExpressionTerminalVisited(ExpressionResult expressionResult)
        {
            ExpressionResult = expressionResult;
        }

        public ExpressionResult ExpressionResult { get; }
    }
}