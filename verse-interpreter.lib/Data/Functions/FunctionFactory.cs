using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			bluePrint.Parameters.ForEach(p =>
			{
				p = ClearedValues(p);
			});

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
