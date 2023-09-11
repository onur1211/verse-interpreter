using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data
{
	public interface IScope<T>
	{
		Dictionary<int, IScope<T>> SubScope { get; }

		LookupManager LookupManager { get; }

		void AddScopedVariable(Variable variable);
	}
}
