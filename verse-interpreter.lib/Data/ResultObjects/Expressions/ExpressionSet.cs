namespace verse_interpreter.lib.Data.ResultObjects.Expressions
{
    public class ExpressionSet
    {
        public List<List<ExpressionResult>> Expressions { get; set; } = null!;

        public ExpressionSet(List<List<ExpressionResult>> expressionResults)
        {
            this.Expressions = expressionResults;
        }
    }
}
