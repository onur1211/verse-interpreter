using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Expressions
{
	public class ForExpression : IExpression<ForExpression>
	{
		public Func<ForExpression>? PostponedExpression { get; set; }

		public VerseCollection? Collection { get; set; }
	}
}
