using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Data.ResultObjects
{
	public class IfParseResult : IExpression<IfParseResult>
	{
		public LogicalExpression LogicalExpression { get; set; } = null!;
		public List<Verse.BlockContext> ThenBlock { get; set; } = null!;
		public List<Verse.BlockContext> ElseBlock { get; set; } = null!;
		public Variable? ScopedVariable { get; set; }
		public Func<IfParseResult>? PostponedExpression { get; set; }
	}
}