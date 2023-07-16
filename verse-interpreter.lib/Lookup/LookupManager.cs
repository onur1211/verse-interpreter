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
        private List<string> valueLessVariables;
        private PropertyResolver propertyResolver;

        public LookupManager()
        {
            this.lookupTable = new LookupTable<Variable>();
            this.valueLessVariables = new List<string>();
        }

        public event EventHandler<VariableBoundEventArgs>? VariableBound;

        public void AddVariable(Variable variable)
        {
            // Check if variable is already in a lookup table (which means the variable was already declared once).
            // If true then update the value of the variable.
            if (IsVariable(variable.Name))
            {
                this.UpdateVariable(variable);
                return;
            }

            if (!variable.HasValue())
            {
                this.valueLessVariables.Add(variable.Name);
            }

            // Add the new variable to table.
            this.lookupTable.Table.Add(variable.Name, variable);
            this.FireVariableBound(variable);
        }

        public void UpdateVariable(Variable variable)
        {
            // Check if variable is in the lookup table.
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

            // Replace old variable entry with new one.
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

        public Variable GetMemberVariable(CustomType instance, string variableName, string propertyName)
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

