using verse_interpreter.lib.Data.Expressions;

namespace verse_interpreter.lib.Data.ResultObjects
{
    public class LogicalExpression
    {
        public LogicalOperators? LogicalOperator { get; set; }
        public LogicalExpression? Next { get; set; }
        public List<List<ExpressionResult>>? Expressions { get; set; }
        public bool IsNegated { get; set; }
    }
}