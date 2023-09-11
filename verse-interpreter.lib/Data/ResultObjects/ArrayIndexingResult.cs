namespace verse_interpreter.lib.ParseVisitors
{
	public class ArrayIndexingResult
	{
		public string ArrayIdentifier { get; internal set; } = null!;
		public string Indexer { get; internal set; } = null!;
	}
}