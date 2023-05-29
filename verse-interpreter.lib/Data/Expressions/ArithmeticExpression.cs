using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Factories
{
    [Serializable]
    public class ArithmeticExpression : IExpression<ArithmeticExpression>
    {
        public string StringRepresentation { get; set; } = null!;

        public Nullable<int> ResultValue { get; set; }

        public Func<ArithmeticExpression>? PostponedExpression { get; set; }

        public List<List<ExpressionResult>>? Arguments { get; set; }

        public ArithmeticExpression()
        {
        }

        public ArithmeticExpression Accept()
        {
            return PostponedExpression!.Invoke();
        }
    }
}