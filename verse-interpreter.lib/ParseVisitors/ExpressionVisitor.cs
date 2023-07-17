using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using static verse_interpreter.lib.Grammar.Verse;

namespace verse_interpreter.lib.ParseVisitors
{
    public class ExpressionVisitor : AbstractVerseVisitor<List<List<ExpressionResult>>>
    {
        private List<List<ExpressionResult>> _expressions;
        private readonly PrimaryRuleParser _primaryRuleParser;

        public ExpressionVisitor(ApplicationState applicationState,
                                 PrimaryRuleParser primaryRuleParser) : base(applicationState)
        {
            _expressions = new List<List<ExpressionResult>>();
            ExpressionTerminalVisited += TerminalNodeVisitedCallback;
            _primaryRuleParser = primaryRuleParser;
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

        public void Clean()
        {
            _expressions = new List<List<ExpressionResult>>();
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
            var expressionContext = context.expression();

            if (expressionContext != null)
            {
                this.VisitExpression(expressionContext);
                _expressions.Add(new List<ExpressionResult>());
                return null;
            }

            var expressionResult = _primaryRuleParser.ParsePrimary(context);    
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
