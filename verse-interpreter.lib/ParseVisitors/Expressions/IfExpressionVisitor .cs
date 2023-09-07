using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.ParseVisitors.Expressions;

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
            bool NOTOperator = false;
            List<ComparisonExpression> compExpressions = new List<ComparisonExpression>();

            _generalEvaluator.ComparisonExpressionResolved += (sender, args) =>
            {
                compExpressions.Add(args.Result);
            };

            var logicalExpressionContext = context.logical_expression();
            List<List<List<ExpressionResult>>> expResults = new List<List<List<ExpressionResult>>>();

            if (logicalExpressionContext == null)
            {
                throw new ArgumentNullException(nameof(logicalExpressionContext), "Error: There was no valid logical expression given in the if statement!");
            }

            // Check if there is a NOT (!) operator token.
            if (logicalExpressionContext.NOT() != null)
            {
                NOTOperator = true;
            }

            // Check if there are AND or OR conjunctions and get all expression results.
            // Example: x=1 and y=1
            // => Expression Result 1: x=1
            // => Expression Result 2: y=1
            if (logicalExpressionContext.expression().Length > 1)
            {
                foreach (var exp in logicalExpressionContext.expression())
                {
                    expResults.Add(_expressionVisitor.Visit(exp));
                }

                foreach (var expRes in expResults)
                {
                    _generalEvaluator.ExecuteExpression(expRes);
                }
            }
            else
            {
                var result = _expressionVisitor.Visit(context.logical_expression());
                _generalEvaluator.ExecuteExpression(result);
            }

            if (compExpressions == null)
            {
                return new List<Verse.BlockContext>();
            }

            switch (true)
            {
                case true when logicalExpressionContext.COMMA().Length > 0:
                    return EvaluateAndConjunction(context, compExpressions, NOTOperator);

                case true when logicalExpressionContext.CHOICE().Length > 0:
                    return EvaluateOrConjunction(context, compExpressions, NOTOperator);

                default:
                    return EvaluateDefaultNoConjunction(context, compExpressions, NOTOperator);
            }
        }

        private List<Verse.BlockContext> EvaluateAndConjunction(Verse.If_blockContext context, List<ComparisonExpression> compExpressions, bool NOTOperator)
        {
            // Check if one expression is false
            // If even one expression is false then parse the 'else' block
            foreach (var compResult in compExpressions)
            {
                if (compResult.Value == null)
                {
                    return ParseElseBlock(context);
                }
            }

            // Otherwise parse the 'then' block
            return ParseThenBlock(context);
        }

        private List<Verse.BlockContext> EvaluateOrConjunction(Verse.If_blockContext context, List<ComparisonExpression> compExpressions, bool NOTOperator)
        {
            // Check if at least one expression is true
            // If true then parse the 'then' block
            foreach (var compResult in compExpressions)
            {
                if (compResult.Value != null)
                {
                    return ParseThenBlock(context);
                }
            }

            // Otherwise parse the 'else' block
            return ParseElseBlock(context);
        }

        private List<Verse.BlockContext> EvaluateDefaultNoConjunction(Verse.If_blockContext context, List<ComparisonExpression> compExpressions, bool NOTOperator)
        {
            if (NOTOperator)
            {
                // If the expression is false then parse the 'then' block
                if (compExpressions.First().Value == null)
                {
                    return ParseThenBlock(context);
                }

                // Otherwise parse the 'else' block
                return ParseElseBlock(context);
            }
            else
            {
                // If the expression is true then parse the 'then' block
                if (compExpressions.First().Value != null)
                {
                    return ParseThenBlock(context);
                }

                // Otherwise parse the 'else' block
                return ParseElseBlock(context);
            }
        }

        private List<Verse.BlockContext> ParseThenBlock(Verse.If_blockContext context)
        {
            return _parser.GetBody(context.then_block().body());
        }

        private List<Verse.BlockContext> ParseElseBlock(Verse.If_blockContext context)
        {
            return _parser.GetBody(context.else_block().body());
        }
    }
}
