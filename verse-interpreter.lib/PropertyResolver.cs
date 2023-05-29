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

            return _applicationState.CurrentScope.LookupManager.GetVariable(propertyName);
            // first is variable, second is property identifier ( could be value or other variable type) repeat

        }

        private Variable ResolveProperty(string[] parameters, LookupManager lookupTable)
        {
            var identifier = parameters[0];
            var baseVariable = lookupTable.GetVariable(identifier);
            if (baseVariable.Value.DynamicType == null)
            {
                return baseVariable;
            }
            return ResolveProperty(parameters.Skip(1).ToArray(), baseVariable.Value.DynamicType.LookupManager);
        }
    }
}
