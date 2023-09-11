using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Data.ResultObjects
{
	public class ChoiceResult
	{
		public List<ArrayIndexingResult> IndexingResults { get; set; }
		public List<Variable> Literals { get; set; }
		public ChoiceResult Next { get; set; }

		public ChoiceResult()
		{
			this.IndexingResults = new List<ArrayIndexingResult>();
			this.Literals = new List<Variable>();
			this.Next = null!;
		}
	}
}
