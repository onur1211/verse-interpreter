using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			return _evaluator.Evaluate(expression.Expressions).Value != null;
		}
	}
}
