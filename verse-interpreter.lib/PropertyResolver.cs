using verse_interpreter.lib.Data;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib
{
    public class PropertyResolver
    {
        private readonly ApplicationState _applicationState;

        public PropertyResolver(ApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        public Variable ResolveProperty(string propertyName)
        {
            if (propertyName.Contains("."))
            {
                var parameters = propertyName.Split(".");
                return ResolveProperty(parameters, _applicationState.CurrentScope.LookupManager); ;
            }
            if (propertyName.EndsWith("]"))
            {
                return ResolveArrayAccess(propertyName);
            }

            return _applicationState.CurrentScope.LookupManager.GetVariable(propertyName);
        }

        private Variable ResolveProperty(string[] parameters, LookupManager lookupTable)
        {
            var identifier = parameters[0];

            if (!lookupTable.IsVariable(identifier))
            {
                return null!;
            }

            var baseVariable = lookupTable.GetVariable(identifier);
            if (baseVariable.Value.CustomType == null)
            {
                return baseVariable;
            }
            return ResolveProperty(parameters.Skip(1).ToArray(), baseVariable.Value.CustomType.LookupManager);
        }

        private Variable ResolveArrayAccess(string variableName)
        {
            var identifieres = variableName.Split('[', ']');

            var collection = _applicationState.CurrentScope.LookupManager.GetVariable(identifieres[0]).Value.CollectionVariable.Values;
            int index = Convert.ToInt32(identifieres[1]);

            return collection[index];
        }
    }
}
