using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data.Functions
{
	public class FunctionFactory
	{
		private readonly ApplicationState _applicationState;

		public FunctionFactory(ApplicationState applicationState)
		{
			_applicationState = applicationState;
		}

		public Function GetFunctionInstance(string functionName)
		{
			var bluePrint = _applicationState.GetFunction(functionName);
			Function function = new Function()
			{
			};
			bluePrint.Parameters.ForEach(p =>
			{
				p = ClearedValues(p);
			});
			//Might need change
			bluePrint.LookupManager = new LookupManager();

			return bluePrint;
		}

		private Variable ClearedValues(Variable variable)
		{
			var clearedVariable = new Variable();
			clearedVariable.Name = variable.Name;
			clearedVariable.Value = new ValueObject(variable.Value.TypeData.Name);

			return clearedVariable;
		}
	}
}
