using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors.Expressions
{
    public class LogicalExpressionVisitor : AbstractVerseVisitor<LogicalExpression>
    {
        private readonly Lazy<ExpressionVisitor> _expressionVisitor;
        private LogicalExpression _logicalExpression;
        public LogicalExpressionVisitor(ApplicationState applicationState,
                                        Lazy<ExpressionVisitor> expressionVisitor) : base(applicationState)
        {
            _expressionVisitor = expressionVisitor;
            _logicalExpression = new LogicalExpression();
        }

        public override LogicalExpression VisitLogical_expression([NotNull] Verse.Logical_expressionContext context)
        {
            var current = _logicalExpression;
            while (current.Next != null)
            {
                current = current.Next;
            }

            var and = context.COMMA();
            var or = context.CHOICE();
            if (and.Length > 0)
            {
                current.LogicalOperator = LogicalOperators.AND;
            }
            if (or.Length > 0)
            {
                current.LogicalOperator = LogicalOperators.OR;
            }
            current.Expressions = _expressionVisitor.Value.Visit(context);

            var nextExpression = context.logical_expression();
            if (nextExpression.Length > 0)
            {
                // Can actually never be more than element
                current.Next = Visit(nextExpression.First());
            }

            _logicalExpression = new LogicalExpression();
            return current;
        }

        private void AddToExpression()
        {

        }
    }
}
