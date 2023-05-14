using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class ExpressionVisitor : AbstractVerseVisitor<ExpressionResult>
    {
        List<List<ExpressionResult>> _expressions;

        public ExpressionVisitor(ApplicationState applicationState) : base(applicationState)
        {
            _expressions = new List<List<ExpressionResult>>();
            ExpressionTerminalVisited += TerminalNodeVisitedCallback;
        }

        private event EventHandler<ExpressionTerminalVisited> ExpressionTerminalVisited;

        public event EventHandler<ExpressionParsedSucessfullyEventArgs> ExpressionParsedSucessfully;

        public override ExpressionResult VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            _expressions.Add(new List<ExpressionResult>());
            VisitChildren(context);
            ExpressionParsedSucessfully?.Invoke(this, new ExpressionParsedSucessfullyEventArgs(_expressions));
            _expressions.Clear();
            return new ExpressionResult();
        }

        private void TerminalNodeVisitedCallback(object sender, ExpressionTerminalVisited args)
        {
            _expressions.Last().Add(args.ExpressionResult);
        }

        public override ExpressionResult VisitPrimary([NotNull] Verse.PrimaryContext context)
        {
            Verse.ExpressionContext? expressionContext = context.expression();
            if (expressionContext != null)
            {
                return VisitExpression(expressionContext);
            }
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
