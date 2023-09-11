namespace verse_interpreter.lib.Data.Interfaces
{
	public interface IValidator<T>
	{
		bool IsTypeConformityGiven(T value);
	}
}