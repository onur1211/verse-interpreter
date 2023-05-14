using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;

namespace verse_interpreter.lib.Lookup
{
    public class LookupManager
    {
        private ILookupTable<int?> lookup;

        public LookupManager(ILookupTable<int?> table) 
        {
            this.lookup = table;
        }

        public void Add(DeclarationResult declarationResult) 
        {
            // Create a values list
            List<int?> values = new List<int?>();

            // Check if variable is already in lookup table (which means the variable was already declared once).
            // If true then throw exception.
            if (this.lookup.Table.ContainsKey(declarationResult.Name)) 
            {
                throw new Exception();
            }

            // Get the values from the declaration result and add to lookup.
            values.Add(declarationResult.Value);
            this.lookup.Table.Add(declarationResult.Name, values);
        }

        public List<int?> GetVariableValue(string variableName)
        {
            if (string.IsNullOrEmpty(variableName)) 
            {
                throw new ArgumentNullException();
            }

            // Check if variable is in lookup.
            // If false then the variable does not exist (which means it was never declared).
            if (!this.lookup.Table.ContainsKey(variableName))
            {
                throw new Exception();
            }

            // Get the values of the variable and return it.
            List<int?> values;
            this.lookup.Table.TryGetValue(variableName, out values!);
            return values!;
        }

        public bool IsVariable(string name)
        {
            if (string.IsNullOrEmpty(name)) 
            {
                return false;
            }
            else
            {
                return this.lookup.Table.ContainsKey(name);
            }
        }
    }
}
