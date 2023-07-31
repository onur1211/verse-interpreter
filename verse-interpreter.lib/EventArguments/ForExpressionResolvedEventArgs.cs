using verse_interpreter.lib.Data.Expressions;

namespace verse_interpreter.lib.Evaluation.EvaluationManagement
{
	public class ForExpressionResolvedEventArgs
	{
		public ForExpressionResolvedEventArgs(ForExpression result)
		{
			ForExpression = result;
		}

		public ForExpression ForExpression { get; }
	}
}