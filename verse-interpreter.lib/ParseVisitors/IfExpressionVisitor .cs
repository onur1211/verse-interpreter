
using Antlr4.Runtime.Tree;
using System.Linq.Expressions;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.ParseVisitors
{
    public class IfExpressionVisitor : AbstractVerseVisitor<List<Verse.BlockContext>>
    {
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly BodyParser _parser;
        private readonly GeneralEvaluator _generalEvaluator;

        public IfExpressionVisitor(ApplicationState applicationState,
                                   ExpressionVisitor expressionVisitor,
                                   BodyParser parser,
                                   GeneralEvaluator evaluator) : base(applicationState)
        {
            _expressionVisitor = expressionVisitor;
            _parser = parser;
            _generalEvaluator = evaluator;
            ApplicationState.CurrentScope.LookupManager.VariableBound +=
                _generalEvaluator.Propagator.HandleVariableBound;

            _generalEvaluator.ComparisonExpressionResolved += (sender, args) =>
            {
               var expression = args.Result;
            };
        }

        public override List<Verse.BlockContext> VisitIf_block(Verse.If_blockContext context)
        {
            var result = _expressionVisitor.Visit(context.logical_expression());
            ComparisonExpression expression = null;
            _generalEvaluator.ComparisonExpressionResolved += (sender, args) =>
            {
                expression = args.Result;
            };
            _generalEvaluator.ExecuteExpression(result);

            if (expression == null)
            {
                return new List<Verse.BlockContext>();
            }

            if (expression.Value != null)
            {
                return _parser.GetBody(context.then_block().body());
            }

            return _parser.GetBody(context.else_block().body());
        }
    }
}
