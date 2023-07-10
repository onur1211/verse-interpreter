﻿using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Functions;

namespace verse_interpreter.lib
{
    public class ApplicationState
    {
        public Dictionary<int, IScope<Variable>> Scopes { get; set; }
        public Dictionary<string, DynamicType> Types { get; set; }

        public List<Function> PredefinedFunctions { get; set; }

        public ApplicationState(PredefinedFunctionInitializer initializer)
        {
            Scopes = new Dictionary<int, IScope<Variable>>
            {
                { 1, new CurrentScope(1) }
            };

            Types = new Dictionary<string, DynamicType>();

            CurrentScopeLevel = 1;

            WellKnownTypes = new List<string>()
            {
                "int", "string", "false?", "dynamic", "collection"
            };

            PredefinedFunctions = initializer.GetPredefinedFunctions();
        }

        public List<string> WellKnownTypes { get; }

        public int CurrentScopeLevel { get; set; }  

        public IScope<Variable> CurrentScope { get { return Scopes[CurrentScopeLevel]; } }
    }
}
