using Antlr4.Runtime;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Lookup
{
    public class LookupManager
    {
        private ILookupTable<Variable> lookupTable;

        public LookupManager()
        {
            this.lookupTable = new LookupTable<Variable>();
        }

        public void AddVariable(Variable variable)
        {
            // Check if variable is already in a lookup table (which means the variable was already declared once).
            // If true then throw exception.
            if (this.lookupTable.Table.ContainsKey(variable.Name))
            {
                throw new VariableAlreadyExistException(variable.Name);
            }

            this.lookupTable.Table.Add(variable.Name, variable);
        }

        public void UpdateVariable(Variable variable)
        {
            if (!IsVariable(variable.Name))
            {
                throw new VariableDoesNotExistException(variable.Name);
            }

            this.lookupTable.Table[variable.Name] = variable;
        }

        public Variable GetVariable(string variableName) 
        {
            if (!IsVariable(variableName))
            {
                throw new VariableDoesNotExistException(variableName);
            }

            return this.lookupTable.Table[variableName];
        }

        public bool IsVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            else
            {
                return this.lookupTable.Table.ContainsKey(name);
            }
        }
    }
}
