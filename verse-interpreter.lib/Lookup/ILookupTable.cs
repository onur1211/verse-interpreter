namespace verse_interpreter.lib.Lookup
{
	public interface ILookupTable<T>
	{
		public Dictionary<string, T> Table { get; set; }
	}
}
