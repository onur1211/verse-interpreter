using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Data.Expressions
{
    [Serializable]

    public class StringExpression : IExpression<StringExpression>
    {
        public Func<StringExpression> PostponedExpression { get; set; } = null!;

        public string Value { get; internal set; } = null!;

        public List<List<ExpressionResult>> Arguments { get; set; }

        public StringExpression()
        {
            Arguments = new List<List<ExpressionResult>>();
        }
    }
}