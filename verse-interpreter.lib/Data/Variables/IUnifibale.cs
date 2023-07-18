namespace verse_interpreter.lib.Data
{
	public interface IUnifiable<T>
	{
		bool CanUnify(T value);
	}
}