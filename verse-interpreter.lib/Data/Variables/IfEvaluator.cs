using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.Evaluators;

namespace verse_interpreter.lib.Data.Variables
{
    public class IfEvaluator : IEvaluator<bool, IfParseResult>
    {
        private readonly IEvaluator<ComparisonExpression, List<List<ExpressionResult>>> _evaluator;

        public IfEvaluator(IEvaluator<ComparisonExpression, List<List<ExpressionResult>>> evaluator)
        {
            _evaluator = evaluator;
        }

        public bool AreVariablesBoundToValue(IfParseResult input)
        {
            throw new NotImplementedException();
        }

        public bool Evaluate(IfParseResult input)
        {
            if (input.ScopedVariable != null)
            {
                return !(input.ScopedVariable.Value == ValueObject.False);
            }

            return HandleLogicalExpressions(input.LogicalExpression);
        }

        private bool HandleLogicalExpressions(LogicalExpression expression)
        {
            List<ComparisonExpression> results = new List<ComparisonExpression>();
            var current = expression;

            var evaluatedExpression = _evaluator.Evaluate(current.Expressions!);

            if (evaluatedExpression.IntValue != null && current.IsNegated ||
                evaluatedExpression.StringValue != null && current.IsNegated)
            {
                results.Add(new ComparisonExpression()
                {
                    IntValue = null,
                    StringValue = null
                });
            }

            if (evaluatedExpression.IntValue == null && current.IsNegated ||
                evaluatedExpression.StringValue == null && current.IsNegated)
            {
                results.Add(new ComparisonExpression()
                {
                    IntValue = 0,
                    StringValue = ""
                });
            }

            if (evaluatedExpression.IntValue == null && !current.IsNegated ||
                evaluatedExpression.StringValue == null && !current.IsNegated)
            {
                results.Add(evaluatedExpression);
            }

            while (current.Next != null)
            {
                current = current.Next;
                evaluatedExpression = _evaluator.Evaluate(current.Expressions!);

                if (evaluatedExpression.IntValue != null && current.IsNegated ||
                   evaluatedExpression.StringValue != null && current.IsNegated)
                {
                    results.Add(new ComparisonExpression()
                    {
                        IntValue = null,
                        StringValue = null
                    });
                    continue;
                }
                if (evaluatedExpression.IntValue == null && !current.IsNegated ||
                    evaluatedExpression.StringValue == null && !current.IsNegated)
                {
                    results.Add(new ComparisonExpression()
                    {
                        IntValue = 0,
                        StringValue = ""
                    });
                    continue;
                }

                results.Add(evaluatedExpression);
            }

            if (expression.LogicalOperator == Expressions.LogicalOperators.OR)
            {
                return results.Any(x => x.IntValue != null || x.StringValue != null);
            }
            if (expression.LogicalOperator == Expressions.LogicalOperators.AND)
            {
                return results.All(x => x.IntValue != null || x.StringValue != null);
            }

            return results.FirstOrDefault()!.StringValue != null || results.FirstOrDefault()!.IntValue != null;
        }
    }
}
