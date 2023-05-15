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
        private ILookupTable<int?> intLookupTable;
        private ILookupTable<string> stringLookupTable;

        public LookupManager(ILookupTable<int?> intTable,
                             ILookupTable<string> stringTable)
        {
            this.intLookupTable = intTable;
            this.stringLookupTable = stringTable;
        }

        public void Add(DeclarationResult declarationResult)
        {
            // Create a values list
            List<int?> intValues = new List<int?>();
            List<string> stringValues = new List<string>();

            // Check if variable is already in lookup table (which means the variable was already declared once).
            // If true then throw exception.
            if (this.intLookupTable.Table.ContainsKey(declarationResult.Name))
            {
                throw new Exception();
            }
            if (this.stringLookupTable.Table.ContainsKey(declarationResult.Name))
            {
                throw new Exception();
            }

            if (declarationResult.TypeName == "int")
            {
                int parsedValue = 0;
                var hasValues = int.TryParse(declarationResult.Value, out parsedValue);
                if (hasValues)
                {
                    this.intLookupTable.Table.Add(declarationResult.Name, intValues);
                    intValues.Add(parsedValue);
                    return;
                }
                this.intLookupTable.Table.Add(declarationResult.Name, null);

            }
            if (declarationResult.TypeName == "string")
            {
                stringValues.Add(declarationResult.Value);
                this.stringLookupTable.Table.Add(declarationResult.Name, stringValues);
            }
            // Get the values from the declaration result and add to lookup.
        }

        public List<string> GetVariableStrings(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException();
            }

            // Check if variable is in lookup.
            // If false then the variable does not exist (which means it was never declared).
            if (!this.stringLookupTable.Table.ContainsKey(variableName))
            {
                return new List<string>();
            }

            // Get the values of the variable and return it.
            List<string> values;
            this.stringLookupTable.Table.TryGetValue(variableName, out values!);
            return values!;
        }

        public List<int?> GetVariableInts(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException();
            }

            // Check if variable is in lookup.
            // If false then the variable does not exist (which means it was never declared).
            if (!this.intLookupTable.Table.ContainsKey(variableName))
            {
                return new List<int?>();
            }

            // Get the values of the variable and return it.
            List<int?> values;
            this.intLookupTable.Table.TryGetValue(variableName, out values!);
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
                return this.intLookupTable.Table.ContainsKey(name) || this.stringLookupTable.Table.ContainsKey(name);
            }
        }
    }
}
