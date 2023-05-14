using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Visitors
{
    public class ExpressionParsedSucessfullyEventArgs : EventArgs
    {
        public ExpressionParsedSucessfullyEventArgs(List<List<ExpressionResult>> expressions)
        {
            Expressions = expressions;
        }

        public List<List<ExpressionResult>> Expressions { get; }
    }
}