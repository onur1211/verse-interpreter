using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data
{
	public class CurrentScope : IScope<Variable>
	{
		private int _level;

		public Dictionary<int, IScope<Variable>> SubScope { get; private set; }

		public LookupManager LookupManager { get; set; }

		public int Level { get { return _level; } set { _level = value; } }

		public CurrentScope(int level)
		{
			LookupManager = new LookupManager();
			SubScope = new Dictionary<int, IScope<Variable>>();
			_level = level;
		}

		public void AddScopedVariable(Variable variable)
		{
			this.LookupManager.AddVariable(variable);
		}
	}
}
