using Antlr4.Runtime.Misc;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class ExpressionVisitor : AbstractVerseVisitor<List<List<ExpressionResult>>>
    {
        private List<List<ExpressionResult>> _expressions;

        public ExpressionVisitor(ApplicationState applicationState,
                                 ExpressionValidator expressionValidator) : base(applicationState)
        {
            _expressions = new List<List<ExpressionResult>>();
            ExpressionTerminalVisited += TerminalNodeVisitedCallback;
        }

        private event EventHandler<ExpressionTerminalVisited> ExpressionTerminalVisited = null!;
        public event EventHandler<ExpressionParsedSucessfullyEventArgs> ExpressionParsedSucessfully = null!;

        public override List<List<ExpressionResult>> VisitExpression([Antlr4.Runtime.Misc.NotNull] Verse.ExpressionContext context)
        {
            // After every recursive call, a new sublist ist created to differentiate between the "scopes" of the expression --> see brackets
            _expressions.Add(new List<ExpressionResult>());

            // When all child nodes have been visited, an event is triggered containing all the unevaluated expressions as the arguments.
            //ExpressionParsedSucessfully?.Invoke(this, new ExpressionParsedSucessfullyEventArgs(_expressions));
            VisitChildren(context);

            return _expressions;
        }

        public override List<List<ExpressionResult>> VisitTerm([Antlr4.Runtime.Misc.NotNull] Verse.TermContext context)
        {
            return base.VisitTerm(context);
        }

        private void TerminalNodeVisitedCallback(object? sender, ExpressionTerminalVisited? args)
        {
            if (_expressions.Count == 0)
            {
                return;
            }
            _expressions.Last().Add(args!.ExpressionResult);
        }

        public override List<List<ExpressionResult>> VisitPrimary([Antlr4.Runtime.Misc.NotNull] Verse.PrimaryContext context)
        {
            Verse.ExpressionContext? expressionContext = context.expression();
            // Checks if the there are any subexpressions --> due to brackets for instance
            if (expressionContext != null)
            {
                VisitExpression(expressionContext);
                _expressions.Add(new List<ExpressionResult>());
                return null;
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
                IntegerValue = value,
                ValueIdentifier = identifier,
            };
            // When the instance is finalized the event is triggered to append it to the final result set
            this.ExpressionTerminalVisited?.Invoke(this, new ExpressionTerminalVisited(expressionResult));
            return base.VisitChildren(context);
        }

        public override List<List<ExpressionResult>> VisitOperator([Antlr4.Runtime.Misc.NotNull] Verse.OperatorContext context)
        {
            var operatorResult = new ExpressionResult
            {
                Operator = context.GetText(),
            };

            this.ExpressionTerminalVisited?.Invoke(this, new ExpressionTerminalVisited(operatorResult));
            return base.VisitChildren(context);
        }
    }
}
