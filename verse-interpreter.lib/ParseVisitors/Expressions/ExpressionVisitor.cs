using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.ParseVisitors.Functions;
using static verse_interpreter.lib.Grammar.Verse;

namespace verse_interpreter.lib.ParseVisitors.Expressions
{
    public class ExpressionVisitor : AbstractVerseVisitor<List<List<ExpressionResult>>>
    {
        private List<List<ExpressionResult>> Expressions
        {
            get { return _stack.Peek(); }
            set
            {
                if (_stack.Count > 0)
                {
                    _stack.Pop();
                }

                _stack.Push(value);
            }
        }
        private Stack<List<List<ExpressionResult>>> _stack;
        private readonly Lazy<PrimaryRuleParser> _primaryRuleParser;
        private readonly Lazy<FunctionCallVisitor> _functionCallVisitor;

        public ExpressionVisitor(ApplicationState applicationState,
                                 Lazy<PrimaryRuleParser> primaryRuleParser,
                                 Lazy<FunctionCallVisitor> functionCallVisitor) : base(applicationState)
        {
            _stack = new Stack<List<List<ExpressionResult>>>();
            ExpressionTerminalVisited += TerminalNodeVisitedCallback;
            _primaryRuleParser = primaryRuleParser;
            _functionCallVisitor = functionCallVisitor;
        }

        private event EventHandler<ExpressionTerminalVisited> ExpressionTerminalVisited = null!;
        public event EventHandler<ExpressionParsedSucessfullyEventArgs>? ExpressionParsedSucessfully;

        public override List<List<ExpressionResult>> VisitExpression([NotNull] ExpressionContext context)
        {
            if (ApplicationState.CurrentScopeLevel > _stack.Count)
            {
                _stack.Push(new List<List<ExpressionResult>>());
            }

            // After every recursive call, a new sublist ist created to differentiate between the "scopes" of the expression --> see brackets
            Expressions.Add(new List<ExpressionResult>());

            // When all child nodes have been visited, an event is triggered containing all the unevaluated expressions as the arguments.
            //ExpressionParsedSucessfully?.Invoke(this, new ExpressionParsedSucessfullyEventArgs(_expressions));
            VisitChildren(context);

            return _stack.Pop();
        }

        public void Clean()
        {
            Expressions = new List<List<ExpressionResult>>();
        }

        public override List<List<ExpressionResult>> VisitTerm([NotNull] TermContext context)
        {
            return base.VisitTerm(context);
        }

        private void TerminalNodeVisitedCallback(object? sender, ExpressionTerminalVisited? args)
        {
            if (Expressions.Count == 0)
            {
                Expressions.Add(new List<ExpressionResult>());
            }
            Expressions.Last().Add(args!.ExpressionResult);
        }

        public override List<List<ExpressionResult>> VisitPrimary([NotNull] PrimaryContext context)
        {
            var expressionContext = context.expression();
            if (expressionContext != null)
            {
                VisitExpression(expressionContext);
                Expressions.Add(new List<ExpressionResult>());
                return null!;
            }
            var expressionResult = _primaryRuleParser.Value.ParsePrimary(context);
            // When the instance is finalized the event is triggered to append it to the final result set
            ExpressionTerminalVisited?.Invoke(this, new ExpressionTerminalVisited(expressionResult));
            return base.VisitChildren(context);
        }

        public override List<List<ExpressionResult>> VisitOperator([NotNull] OperatorContext context)
        {
            var operatorResult = new ExpressionResult
            {
                Operator = context.GetText(),
            };

            ExpressionTerminalVisited?.Invoke(this, new ExpressionTerminalVisited(operatorResult));
            return base.VisitChildren(context);
        }

        public override List<List<ExpressionResult>> VisitFunction_call([NotNull] Function_callContext context)
        {
            var result = _functionCallVisitor.Value.Visit(context);
            if (Expressions.Count == 0)
            {
                Expressions.Add(new List<ExpressionResult>());
            }
            Expressions.Last().Add(new ExpressionResult()
            {
                IntegerValue = result.ArithmeticExpression?.ResultValue,
                StringValue = result.StringExpression?.Value!,
                TypeName = result.StringExpression != null ? "string" : "int"
            });
            return base.VisitChildren(context);
        }
    }
}