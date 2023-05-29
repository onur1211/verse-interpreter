
using Antlr4.Runtime.Tree;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.ParseVisitors
{
    public class IfExpressionVisitor : AbstractVerseVisitor<List<Verse.BlockContext>>
    {
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly BodyParser _parser;
        private readonly ComparisonEvaluator _evaluator;

        public IfExpressionVisitor(ApplicationState applicationState,
                                   ExpressionVisitor expressionVisitor,
                                   BodyParser parser,
                                   ComparisonEvaluator evaluator) : base(applicationState)
        {
            _expressionVisitor = expressionVisitor;
            _parser = parser;
            _evaluator = evaluator;
        }

        public override List<Verse.BlockContext> VisitIf_block(Verse.If_blockContext context)
        {
            var result = _expressionVisitor.Visit(context.expression());
            var yieldedValue = _evaluator.Evaluate(result);
            if (yieldedValue.Value != null)
            {
                return _parser.GetBody(context.then_block().body());
            }

            return _parser.GetBody(context.else_block().body());
        }
    }
}
