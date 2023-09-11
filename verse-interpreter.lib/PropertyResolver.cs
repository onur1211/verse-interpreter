using verse_interpreter.lib.Data;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib
{
	/// <summary>
	/// Resolves properties and array accesses within the application state using a lookup manager.
	/// </summary>
	public class PropertyResolver
	{
		private readonly ApplicationState _applicationState;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyResolver"/> class.
		/// </summary>
		/// <param name="applicationState">The application state containing the lookup manager.</param>
		public PropertyResolver(ApplicationState applicationState)
		{
			_applicationState = applicationState;
		}

		/// <summary>
		/// Resolves the specified property name within the current scope's lookup manager.
		/// </summary>
		/// <param name="propertyName">The name of the property to resolve.</param>
		/// <returns>The resolved variable corresponding to the property name.</returns>
		public Variable ResolveProperty(string propertyName)
		{
			// A dot indicates that a member of a type is accessed and it therefor must be handled differently.
			if (propertyName.Contains("."))
			{
				var parameters = propertyName.Split(".");
				return ResolveProperty(parameters, _applicationState.CurrentScope.LookupManager);
			}
			if (propertyName.EndsWith("]"))
			{
				return ResolveIndexing(propertyName);
			}

			return _applicationState.CurrentScope.LookupManager.GetVariable(propertyName);
		}

		/// <summary>
		/// Recursively resolves the specified property parameters using the provided lookup manager.
		/// </summary>
		/// <param name="parameters">The array of property parameters.</param>
		/// <param name="lookupTable">The lookup manager to use for property resolution.</param>
		/// <returns>The resolved variable corresponding to the property parameters.</returns>
		private Variable ResolveProperty(string[] parameters, LookupManager lookupTable)
		{
			var identifier = parameters[0];

			if (!lookupTable.IsVariable(identifier))
			{
				return null!;
			}

			var baseVariable = lookupTable.GetVariable(identifier);
			if (!baseVariable.Value.CustomType.HasValue)
			{
				return baseVariable;
			}
			return ResolveProperty(parameters.Skip(1).ToArray(), baseVariable.Value.CustomType.Value.LookupManager);
		}

		/// <summary>
		/// Resolves the array access for the specified variable name.
		/// </summary>
		/// <param name="variableName">The name of the variable with array access.</param>
		/// <returns>The resolved variable corresponding to the array access.</returns>
		private Variable ResolveIndexing(string variableName)
		{
			var identifieres = variableName.Split('[', ']');
			var variable = ResolveProperty(identifieres[0]);

			if (variable.Value.StringValue != null)
			{
				return ResolveStringAccess(variable, identifieres[1]);
			}
			if (variable.Value.CollectionVariable != null)
			{
				return ResolveArrayAccess(variable, identifieres[1]);
			}

			throw new NotImplementedException($"This kind of indexing is currently not supported! \"{variableName}\"");
		}

		private Variable ResolveArrayAccess(Variable variable, string indexer)
		{
			var collection = variable.Value.CollectionVariable.Values;
			int index;
			bool isNumber = int.TryParse(indexer, out index);
			if (!isNumber)
			{
				index = ResolveProperty(indexer).Value.IntValue!.Value;
			}
			if (collection.Count <= index)
			{
				return new Variable()
				{
					Value = ValueObject.False
				};
			}

			return collection[index];
		}

		private Variable ResolveStringAccess(Variable variable, string indexer)
		{
			var stringVariable = variable.Value.StringValue;
			int index;
			bool isNumber = int.TryParse(indexer, out index);
			if (!isNumber)
			{
				index = ResolveProperty(indexer).Value.IntValue!.Value;
			}
			if (stringVariable.Length <= index)
			{
				return new Variable()
				{
					Value = ValueObject.False
				};
			}
			return new Variable()
			{
				Value = new ValueObject("string", stringVariable[index].ToString())
			};
		}
	}
}
