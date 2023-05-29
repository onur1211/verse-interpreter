using System;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Lookup.EventArguments;

namespace verse_interpreter.lib.Lookup
{
    public class LookupManager
    {
        private ILookupTable<Variable> lookupTable;
        private ILookupTable<Function> lookupFunctions;
        private List<string> valueLessVariables;
        private PropertyResolver propertyResolver;

        public LookupManager()
        {
            this.lookupTable = new LookupTable<Variable>();
            this.lookupFunctions = new LookupTable<Function>();
            this.valueLessVariables = new List<string>();
        }

        public event EventHandler<VariableBoundEventArgs>? VariableBound;

        public void AddVariable(Variable variable)
        {
            // Check if variable is already in a lookup table (which means the variable was already declared once).
            // If true then throw exception.
            if (IsVariable(variable.Name) && this.lookupTable.Table[variable.Name].Value.TypeName == variable.Value.TypeName)
            {
                this.UpdateVariable(variable);
                return;
            }
            if (!variable.HasValue())
            {
                this.valueLessVariables.Add(variable.Name);
            }

            // Add variable to table.
            this.lookupTable.Table.Add(variable.Name, variable);
            this.FireVariableBound(variable);
        }

        public void AddFunction(Function function)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }
            if (lookupFunctions.Table.ContainsKey(function.FunctionName))
            {
                throw new VariableAlreadyExistException(function.FunctionName);
            }

            lookupFunctions.Table.Add(function.FunctionName, function);
        }

        public Function GetFunction(string name)
        {
            if (!lookupFunctions.Table.ContainsKey(name))
            {
                throw new UnknownFunctionException(name);
            }

            return lookupFunctions.Table[name].GetInstance();
        }

        public void UpdateVariable(Variable variable)
        {
            // Check if variable is in the lookupn table.
            // If false then throw exception.
            if (!IsVariable(variable.Name))
            {
                throw new VariableDoesNotExistException(variable.Name);
            }
            if (variable.HasValue())
            {
                this.valueLessVariables.Remove(variable.Name);
                this.lookupTable.Table[variable.Name] = variable;
                this.FireVariableBound(variable);
                return;
            }

            // Replace old entry with new one.
            this.lookupTable.Table[variable.Name] = variable;
        }

        public Variable GetVariable(string variableName)
        {
            // Check if variable is in the lookupn table.
            // If false then throw exception.
            if (!IsVariable(variableName))
            {
                throw new VariableDoesNotExistException(variableName);
            }

            // Return the variable from the table.
            return this.lookupTable.Table[variableName];
        }

        public List<Variable> GetAllVariables()
        {
            return lookupTable.Table.Values.ToList();
        }

        public Variable GetMemberVariable(DynamicType instance, string variableName, string propertyName)
        {
            if (!IsVariable(variableName))
            {
                throw new VariableDoesNotExistException(variableName);
            }

            return instance.LookupManager.lookupTable.Table[propertyName];
        }

        public bool IsVariable(string name)
        {
            return !string.IsNullOrEmpty(name) && this.lookupTable.Table.ContainsKey(name);
        }

        public bool HasValue(string variableName)
        {
            if (!IsVariable(variableName))
            {
                return false;
            }

            return this.lookupTable.Table[variableName].HasValue();
        }

        protected virtual void FireVariableBound(Variable variable)
        {
            this.VariableBound?.Invoke(this, new VariableBoundEventArgs(variable));
        }
    }
}

