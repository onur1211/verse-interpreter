using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib
{
	public class ApplicationState
	{
		public Dictionary<int, IScope<Variable>> Scopes { get; set; }
		public Dictionary<string, CustomType> Types { get; set; }

		public List<Function> PredefinedFunctions { get; set; }

		public Dictionary<string, Function> Functions { get; set; }

		public ApplicationState(PredefinedFunctionInitializer initializer)
		{
			Scopes = new Dictionary<int, IScope<Variable>>
			{
				{ 1, new CurrentScope(1) }
			};

			Types = new Dictionary<string, CustomType>();
			Functions = new Dictionary<string, Function>();

			CurrentScopeLevel = 1;

			WellKnownTypes = new List<TypeData>()
			{
				new TypeData("int"),
				new TypeData("string"),
				new TypeData("int[]"),
				new TypeData("string[]"),
				new TypeData("custom"),
				new TypeData("collection")
			};

			PredefinedFunctions = initializer.GetPredefinedFunctions();
		}

		public List<TypeData> WellKnownTypes { get; }

		public int CurrentScopeLevel { get; set; }

		public IScope<Variable> CurrentScope { get { return Scopes[CurrentScopeLevel]; } }

		public void AddFunction(Function? function)
		{
			if (function == null)
			{
				throw new ArgumentNullException(nameof(function));
			}
			if (Functions.ContainsKey(function.Value.FunctionName))
			{
				throw new VariableAlreadyExistException(function.Value.FunctionName);
			}

			Functions.Add(function.Value.FunctionName, function.Value);
		}

		public Function GetFunction(string name)
		{
			if (!Functions.ContainsKey(name))
			{
				throw new UnknownFunctionException(name);
			}

			var function = Functions[name].Clone();

			return (Function)function;
		}
	}
}
