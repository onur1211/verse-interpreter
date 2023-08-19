using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib
{

	/// <summary>
	/// Wrapper class that combines the different evaluators in one class
	/// </summary>
	public class EvaluatorWrapper
	{
		public EvaluatorWrapper(IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> arithmeticEvaluator,
								IEvaluator<StringExpression, List<List<ExpressionResult>>> stringEvaluator,
								IEvaluator<ComparisonExpression, List<List<ExpressionResult>>> comparisonEvaluator,
								IEvaluator<ForExpression, ForResult> forEvaluator,
								IEvaluator<bool, IfParseResult> ifParseResultEvaluator)
		{
			ArithmeticEvaluator = arithmeticEvaluator;
			StringEvaluator = stringEvaluator;
			ComparisonEvaluator = comparisonEvaluator;
			ForEvaluator = forEvaluator;
			IfParseResultEvaluator = ifParseResultEvaluator;
		}

		public IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> ArithmeticEvaluator { get; }
		public IEvaluator<StringExpression, List<List<ExpressionResult>>> StringEvaluator { get; }
		public IEvaluator<ComparisonExpression, List<List<ExpressionResult>>> ComparisonEvaluator { get; }
		public IEvaluator<ForExpression, ForResult> ForEvaluator { get; }
		public IEvaluator<bool, IfParseResult> IfParseResultEvaluator { get; }
	}
}
