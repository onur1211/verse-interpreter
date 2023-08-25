using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.ResultObjects.Expressions;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Extensions;

namespace verse_interpreter.lib.Evaluation.Evaluators.ForEvaluation
{
	public class FilterApplyer
	{
		private readonly PropertyResolver _resolver;
		private readonly ApplicationState _state;
		private readonly IEvaluator<ComparisonExpression, List<List<ExpressionResult>>> _evaluator;

		public FilterApplyer(PropertyResolver resolver,
							 ApplicationState state,
							  IEvaluator<ComparisonExpression, List<List<ExpressionResult>>> evaluator)
		{
			_resolver = resolver;
			_state = state;
			_evaluator = evaluator;
		}

		public bool DoesFilterMatch(List<ExpressionSet> filters, Variable returnedValue)
		{
			_state.CurrentScope.LookupManager.UpdateVariable(returnedValue);
			foreach (var filter in filters)
			{	
				var copiedFilter = CopyFilter(filter);
				var res = _evaluator.Evaluate(copiedFilter.Expressions);
                if (res.IntValue == null)
                {
					return false;
                }
            }

			return true;
		}

		private ExpressionSet CopyFilter(ExpressionSet expressionSet)
		{
			ExpressionSet set = new ExpressionSet(new List<List<ExpressionResult>>());
			foreach (var expression in expressionSet.Expressions)
			{
				set.Expressions.Add(new List<ExpressionResult>());
				foreach (var exp in expression)
				{
					set.Expressions.Last().Add(new ExpressionResult()
					{
						IntegerValue = exp.IntegerValue,
						StringValue = exp.StringValue,
						Operator = exp.Operator,
						ValueIdentifier = exp.ValueIdentifier,
					});
				}
			}

			return set;
		}
	}
}
