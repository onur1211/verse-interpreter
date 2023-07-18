using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data.CustomTypes
{
	public class CustomTypeFactory
	{
		private readonly ApplicationState _applicationState;

		public CustomTypeFactory(ApplicationState applicationState)
		{
			_applicationState = applicationState;
		}

		public CustomType GetCustomType(string name)
		{
			CustomType? bluePrint = _applicationState.Types[name];
			if (!bluePrint.HasValue)
			{
				throw new UnknownTypeException($"The specified type is unknown \"{name}\"");
			}
			CustomType customType = new CustomType()
			{
				ConstructorName = bluePrint.Value.ConstructorName,
				Name = bluePrint.Value.Name,
				LookupManager = new LookupManager()
			};

			foreach (var element in bluePrint.Value.LookupManager.GetAllVariables())
			{
				customType.LookupManager.AddVariable(ClearedValues(element));
			}

			return customType;
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
