using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Data.ResultObjects
{
	public class CollectionParseResult
	{
		public CollectionParseResult()
		{
			this.ValueElements = new Dictionary<int, Verse.Value_definitionContext>();
			this.DeclarationElements = new Dictionary<int, Verse.DeclarationContext>();
			this.VariableElements = new Dictionary<int, string>();
		}

		public Dictionary<int, Verse.Value_definitionContext> ValueElements { get; set; }

		public Dictionary<int, Verse.DeclarationContext> DeclarationElements { get; set; }

		public Dictionary<int, string> VariableElements { get; set; }

		public int TotalElements { get; set; }
	}
}
