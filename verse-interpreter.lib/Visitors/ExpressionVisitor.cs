using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class ExpressionVisitor : AbstractVerseVisitor<ExpressionResult>
    {
        private List<List<ExpressionResult>> _expressions;

        public ExpressionVisitor(ApplicationState applicationState) : base(applicationState)
        {
            _expressions = new List<List<ExpressionResult>>();
            ExpressionTerminalVisited += TerminalNodeVisitedCallback;
        }

        private event EventHandler<ExpressionTerminalVisited> ExpressionTerminalVisited = null!;
        public event EventHandler<ExpressionParsedSucessfullyEventArgs> ExpressionParsedSucessfully = null!;

        public override ExpressionResult VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            // After every recursive call, a new sublist ist created to differentiate between the "scopes" of the expression --> see brackets
            _expressions.Add(new List<ExpressionResult>());
            VisitChildren(context);
            if(_expressions.Count > 0)
            {
                // When all child nodes have been visited, an event is triggered containing all the unevaluated expressions as the arguments.
                ExpressionParsedSucessfully?.Invoke(this, new ExpressionParsedSucessfullyEventArgs(_expressions));
            }
            _expressions.Clear();
            return new ExpressionResult();
        }

        private void TerminalNodeVisitedCallback(object? sender, ExpressionTerminalVisited? args)
        {
            if(_expressions.Count == 0)
            {
                return;
            }
            _expressions.Last().Add(args!.ExpressionResult);
        }

        public override ExpressionResult VisitPrimary([NotNull] Verse.PrimaryContext context)
        {
            Verse.ExpressionContext? expressionContext = context.expression();
            // Checks if the there are any subexpressions --> due to brackets for instance
            if (expressionContext != null)
            {
                return VisitExpression(expressionContext);
            }

            // Fetches the value / identifer from the current node
            Nullable<int> value = null;
            string identifier = string.Empty;
            var fetchedValue = context.INT();
            var fetchedIdentifier = context.ID();

            if (fetchedValue != null)
            {
                value = Convert.ToInt32(fetchedValue.ToString());
            }
            if (fetchedIdentifier != null)
            {
                identifier = fetchedIdentifier.GetText();
            }

            var expressionResult = new ExpressionResult()
            {
                Value = value,
                ValueIdentifier = identifier,
            };
            // When the instance is finalized the event is triggered to append it to the final result set
            this.ExpressionTerminalVisited?.Invoke(this, new ExpressionTerminalVisited(expressionResult));
            return this.VisitChildren(context);
        }

        public override ExpressionResult VisitOperator([NotNull] Verse.OperatorContext context)
        {
            var operatorResult = new ExpressionResult
            {
                Operator = context.GetText(),
            };

            this.ExpressionTerminalVisited(this, new Visitors.ExpressionTerminalVisited(operatorResult));
            return this.VisitChildren(context);
        }
    }
}
