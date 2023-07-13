using verse_interpreter.lib.Lookup;
using static verse_interpreter.lib.Grammar.Verse;

namespace verse_interpreter.lib.Data.Functions
{
	public struct Function : IScope<Variable>, ICloneable
    {
        public Function()
        {
            Parameters = new List<Variable>();
            LookupManager = new LookupManager();
            SubScope = new Dictionary<int, IScope<Variable>>();
            StatelessFunctionCall = null;
        }

        public string FunctionName { get; set; } = null!;

        public string ReturnType { get; set; } = null!;

        public int ParameterCount { get { return Parameters.Count; } }

        public List<BlockContext> FunctionBody { get; set; } = null!;

        public List<Variable> Parameters { get; set; } = null!;

        public Dictionary<int, IScope<Variable>> SubScope { get; }

        public LookupManager LookupManager { get; set; }

        public Action<string>? StatelessFunctionCall { get; set; }

        public void AddScopedVariable(Variable variable)
        {
            LookupManager.AddVariable(variable);
        }

		public object Clone()
		{
			Function function = new Function();
			function.FunctionName = FunctionName;
			function.Parameters = Parameters;
			function.ReturnType = ReturnType;
			function.FunctionBody = FunctionBody;
			return function;
		}
    }
}