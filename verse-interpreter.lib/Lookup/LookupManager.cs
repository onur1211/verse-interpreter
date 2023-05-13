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
        private LookupTable<int?> lookup;

        public LookupManager(LookupTable<int?> table) 
        {
            this.lookup = table;
        }

        public void Add(DeclarationResult declarationResult) 
        {
            // Create a values list
            List<int?> values = new List<int?>();

            // Check if variable is already in lookup (which means the variable was already declared once).
            // If true then remove the old entry of the variable and make a new one.
            if (this.lookup.Table.ContainsKey(declarationResult.Name)) 
            {
                this.lookup.Table.Remove(declarationResult.Name);
                values.Add(declarationResult.Value);
                this.lookup.Table.Add(declarationResult.Name, values);
                return;
            }

            // Get the values from the declaration result and add to lookup.
            values.Add(declarationResult.Value);
            this.lookup.Table.Add(declarationResult.Name, values);
        }

        public List<int?> GetVariableValue(string variableName)
        {
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
    }
}
