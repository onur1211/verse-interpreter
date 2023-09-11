using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Evaluation.Evaluators
{
	public class ComparisonExpression : IExpression<ComparisonExpression>
	{
		public Func<ComparisonExpression>? PostponedExpression { get; set; }

		public int? IntValue { get; set; }
		public string? StringValue { get; set; }

		public List<List<ExpressionResult>>? Arguments { get; set; }
	}
}
